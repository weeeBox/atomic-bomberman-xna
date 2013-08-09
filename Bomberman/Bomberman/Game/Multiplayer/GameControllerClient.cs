using System;
using System.Collections.Generic;
using BomberEngine.Core.Input;
using BomberEngine.Game;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Players.Input;
using Bomberman.Game.Screens;
using Bomberman.Networking;
using Lidgren.Network;
using BomberEngine.Debugging;
using BomberEngine.Core.Visual;
using Bomberman.Game.Elements.Fields;
using BomberEngine.Core.Events;
using Bomberman.Multiplayer;

namespace Bomberman.Game.Multiplayer
{
    public class GameControllerClient : GameControllerNetwork
    {
        private const int SENT_HISTORY_SIZE = 32;
        
        private Player m_localPlayer; // TODO: don't store the reference (multiplayer local players may exist)
        private ClientPacket[] m_sentPackets;
        private int m_lastSentPacketId;

        public GameControllerClient(GameSettings settings)
            : base(settings)
        {
            m_sentPackets = new ClientPacket[SENT_HISTORY_SIZE];
        }

        protected override void OnStart()
        {
            base.OnStart();

            Application.SetWindowTitle("Client");

            RequestFieldState();

            RegisterNotification(NetworkNotifications.ConnectedToServer, ConnectedToServerNotification);
            RegisterNotification(NetworkNotifications.DisconnectedFromServer, DisconnectedFromServerNotification);
        }

        protected override void OnStop()
        {
            base.OnStop();
            GetMultiplayerManager().RemoveServerMessageDelegates(this);
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (game != null)
            {
                SendClientPacket(m_localPlayer);
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Server messages delegates

        private void OnFieldStateReceived(Client client, NetworkMessageId messageId, NetIncomingMessage message)
        {
            game = new Game(MultiplayerMode.Client);

            SetupField(settings.scheme);

            ReadFieldState(message);

            gameScreen = new GameScreen();

            m_localPlayer = null;
            List<Player> players = game.GetPlayers().list;
            for (int i = 0; i < players.Count; ++i)
            {
                if (players[i].input == null)
                {
                    m_localPlayer = players[i];
                    m_localPlayer.SetPlayerInput(InputMapping.CreatePlayerInput(InputType.Keyboard1));
                    m_localPlayer.input.IsActive = true; // TODO: handle console
                    break;
                }
            }

            Debug.Assert(m_localPlayer != null);
            m_localPlayer.connection = client.GetServerConnection();

            StartScreen(gameScreen);
            gameScreen.AddDebugView(new NetworkTraceView(client.GetServerConnection()));
            gameScreen.AddDebugView(new LocalPlayerView(m_localPlayer));

            GetMultiplayerManager().RemoveServerMessageDelegate(messageId, OnFieldStateReceived);
            GetMultiplayerManager().AddServerMessageDelegate(NetworkMessageId.ServerPacket, OnServerPacketReceived);
        }

        private void OnServerPacketReceived(Client client, NetworkMessageId messageId, NetIncomingMessage message)
        {   
            if (game != null)
            {
                ServerPacket packet = ReadServerPacket(message);
                m_localPlayer.lastAckPacketId = packet.lastAckClientPacketId;

                if (!CVars.sv_dumbClient.boolValue)
                {
                    ReplayPlayerActions(m_localPlayer);
                }
            }
        }

        private void ReplayPlayerActions(Player player)
        {
            float delta = Application.frameTime;
            player.input.Reset();

            for (int id = player.lastAckPacketId; id <= m_lastSentPacketId; ++id)
            {
                ClientPacket packet = GetPacket(id);
                int actions = packet.actions;
                int actionsCount = (int)PlayerAction.Count;
                for (int i = 0; i < actionsCount; ++i)
                {
                    player.input.SetActionPressed(i, (actions & (1 << i)) != 0);
                }

                player.UpdateMoving(delta);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        private void ConnectedToServerNotification(Notification notification)
        {
            // TODO
        }

        private void DisconnectedFromServerNotification(Notification notification)
        {
            // TODO
        }

        //////////////////////////////////////////////////////////////////////////////

        private void SendClientPacket(Player player)
        {
            int actions = 0;
            int actionsCount = (int)PlayerAction.Count;
            for (int i = 0; i < actionsCount; ++i)
            {
                if (player.input.IsActionPressed(i))
                {
                    actions |= 1 << i;
                }
            }

            player.lastSentPacketId = ++m_lastSentPacketId;

            ClientPacket packet;
            packet.id = player.lastSentPacketId;
            packet.lastAckServerPacketId = player.lastAckPacketId;
            packet.timeStamp = (float)NetTime.Now;
            packet.actions = actions;

            NetOutgoingMessage msg = CreateMessage(NetworkMessageId.ClientPacket);
            WriteClientPacket(msg, ref packet);
            SendMessage(msg);

            PushPacket(ref packet);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Field state

        private void RequestFieldState()
        {
            GetMultiplayerManager().AddServerMessageDelegate(NetworkMessageId.FieldState, OnFieldStateReceived);

            SendMessage(NetworkMessageId.FieldState, NetDeliveryMethod.ReliableSequenced);
            StartConnectionScreen(OnRequestFieldStateCancelled, "Waiting for the server...");
        }

        private void OnRequestFieldStateCancelled()
        {
            throw new NotImplementedException();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Packet history

        private void PushPacket(ref ClientPacket packet)
        {
            int index = packet.id % SENT_HISTORY_SIZE;
            m_sentPackets[index] = packet;
        }

        private ClientPacket GetPacket(int id)
        {
            int index = id % SENT_HISTORY_SIZE;
            Debug.Assert(m_sentPackets[index].id == id);
            return m_sentPackets[index];
        }

        #endregion

        private class NetworkTraceView : View
        {
            private NetConnection m_connection;

            private TextView m_roundTripView;
            private TextView m_remoteTimeOffsetView;
            private TextView m_sentPacketsView;
            private TextView m_receivedPacketsView;
            private TextView m_sentBytesView;
            private TextView m_receivedBytesView;

            public NetworkTraceView(NetConnection connection)
            {
                m_connection = connection;

                m_roundTripView = AddTextView();
                m_remoteTimeOffsetView = AddTextView();
                m_sentPacketsView = AddTextView();
                m_receivedPacketsView = AddTextView();
                m_sentBytesView = AddTextView();
                m_receivedBytesView = AddTextView();

                LayoutVer(0);
                ResizeToFitViews();
            }

            public override void Update(float delta)
            {
                m_roundTripView.SetText("Round trip: " + m_connection.AverageRoundtripTime);
                m_remoteTimeOffsetView.SetText("Remote time offset: " + m_connection.RemoteTimeOffset);

                NetConnectionStatistics stats = m_connection.Statistics;
                m_sentPacketsView.SetText("Sent packet: " + stats.SentPackets);
                m_receivedPacketsView.SetText("Received packet: " + stats.ReceivedPackets);
                m_sentBytesView.SetText("Sent bytes: " + stats.SentBytes);
                m_receivedBytesView.SetText("Received bytes: " + stats.ReceivedBytes);
            }

            private TextView AddTextView()
            {
                TextView view = new TextView(Helper.fontSystem, null);
                AddView(view);
                return view;
            }
        }

        private class LocalPlayerView : View
        {
            private Player m_player;
            private TextView m_cordErrView;
            private TextView m_packetDiffView;

            public LocalPlayerView(Player player)
            {
                m_player = player;

                m_packetDiffView = AddTextView();
                m_cordErrView = AddTextView("px: 0\npy: 0"); // HACK: need to adjust height
                LayoutVer(0);
                ResizeToFitViews();
            }

            private TextView AddTextView(String text = null)
            {
                TextView view = new TextView(Helper.fontSystem, text);
                AddView(view);
                return view;
            }

            public override void Update(float delta)
            {
                m_cordErrView.SetText("px: " + m_player.errDx + "\npy: " + m_player.errDy);
                m_packetDiffView.SetText("packet diff: " + m_player.networkPackageDiff);
            }
        }
    }
}
