using System;
using System.Collections.Generic;
using BomberEngine;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Screens;
using Bomberman.Networking;
using Lidgren.Network;

namespace Bomberman.Game.Multiplayer
{
    public class GameControllerClient : GameControllerNetwork
    {
        private const int SENT_HISTORY_SIZE = 32;
        
        private Player m_localPlayer; // TODO: don't store the reference (multiplayer local players may exist)
        private ClientPacket[] m_sentPackets;
        
        public GameControllerClient(GameSettings settings)
            : base(settings)
        {
            m_sentPackets = new ClientPacket[SENT_HISTORY_SIZE];
        }

        protected override void OnStart()
        {
            base.OnStart();

            Application.SetWindowTitle("Client");

            RegisterNotification(NetworkNotifications.ConnectedToServer,      ConnectedToServerNotification);
            RegisterNotification(NetworkNotifications.DisconnectedFromServer, DisconnectedFromServerNotification);

            StartScreen(new BlockingScreen("Waiting for server..."));
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            SendClientPacket();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Peer messages

        private void SendClientPacket()
        {
            State state = GetState();
            switch (state)
            {
                case State.RoundStart:
                {
                    NetOutgoingMessage msg = CreateMessage(PeerMessageId.RoundStart);
                    WriteRoundStartMessage(msg, m_localPlayer);
                    SendMessage(msg);
                    break;
                }

                case State.Playing:
                {
                    NetOutgoingMessage msg = CreateMessage(PeerMessageId.Playing);
                    WritePlayingMessage(msg, m_localPlayer);
                    SendMessage(msg);
                    break;
                }

                case State.RoundEnd:
                {
                    NetOutgoingMessage msg = CreateMessage(PeerMessageId.RoundEnd);
                    WriteRoundEndMessage(msg, m_localPlayer);
                    SendMessage(msg);
                    break;
                }

                default:
                {
                    Debug.Fail("Unexpected state: " + state);
                    break;
                }
            }
        }

        private void ReplayPlayerActions(Player player)
        {
            Log.d("Diff: " + (m_lastPacketId - player.lastAckPacketId));

            float delta = Application.frameTime;
            player.input.Reset();

            for (int id = player.lastAckPacketId; id <= m_lastPacketId; ++id)
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

        protected override void ReadPeerMessage(Peer peer, PeerMessageId id, NetIncomingMessage msg)
        {
            switch (id)
            {
                case PeerMessageId.RoundStart:
                    ReadRoundStartMessage(peer, msg);
                    break;
                case PeerMessageId.Playing:
                    ReadPlayingMessage(peer, msg);
                    break;
                case PeerMessageId.RoundEnd:
                    ReadRoundEndMessage(peer, msg);
                    break;
                default:
                    Debug.Fail("Unexpected message id: " + id);
                    break;
            }
        }

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

        private void ReadRoundStartMessage(Peer peer, NetIncomingMessage msg)
        {
            SetState(State.RoundStart);

            if (m_localPlayer != null)
            {
                ReadReadyFlags(msg);
            }
            else
            {
                SkipReadyFlags(msg);
            }

            if (m_localPlayer == null || m_localPlayer.needsFieldState)
            {   
                ReadFieldState(peer, msg);
            }
        }

        private void WriteRoundStartMessage(NetBuffer buffer, Player player)
        {
            if (player == null)
            {
                buffer.Write(false); // player is not ready
                buffer.Write(true);  // player needs field state
            }
            else
            {
                buffer.Write(player.IsReady);
                buffer.Write(player.needsFieldState);
            }
        }

        private void ReadPlayingMessage(Peer peer, NetIncomingMessage msg)
        {
            Debug.Assert(game != null);
            Debug.Assert(m_localPlayer != null && m_localPlayer.IsReady);

            SetState(State.Playing);

            m_localPlayer.lastReceivedPackedId = msg.ReadInt32();
            m_localPlayer.lastAckPacketId = msg.ReadInt32();

            ReadPlayingMessage(msg);
            // ReplayPlayerActions(m_localPlayer);
        }

        private void WritePlayingMessage(NetOutgoingMessage msg, Player player)
        {
            Debug.Assert(player != null);

            int actions = 0;
            int actionsCount = (int)PlayerAction.Count;
            for (int i = 0; i < actionsCount; ++i)
            {
                if (player.input.IsActionPressed(i))
                {
                    actions |= 1 << i;
                }
            }

            ClientPacket packet;
            packet.id = ++m_lastPacketId;
            packet.actions = actions;

            msg.Write(packet.id);                   // packet to be acknowledged by server
            msg.Write(player.lastReceivedPackedId); // packet acknowledged by client
            msg.Write(packet.actions, (int)PlayerAction.Count);

            PushPacket(ref packet);
        }

        private void ReadRoundEndMessage(Peer peer, NetIncomingMessage msg)
        {
            Debug.Assert(m_localPlayer != null);

            SetState(State.RoundEnd);

            ReadReadyFlags(msg);

            if (m_localPlayer.needsRoundResults)
            {
                ReadRoundResults(msg);
                m_localPlayer.needsRoundResults = false;
                StartRoundResultScreen();
            }

            if (AllPlayersAreReady())
            {
                if (game.IsGameEnded)
                {
                    SetState(State.GameEnd);
                }
            }
        }

        private void WriteRoundEndMessage(NetBuffer buffer, Player player)
        {
            buffer.Write(player.IsReady);
            buffer.Write(player.needsRoundResults);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Field state

        private void ReadFieldState(Peer peer, NetIncomingMessage message)
        {
            game = new Game(MultiplayerMode.Client);

            SetupField(settings.scheme);
            ReadFieldState(message);

            m_localPlayer = null;

            List<Player> players = game.GetPlayers().list;
            for (int i = 0; i < players.Count; ++i)
            {
                if (players[i].input == null)
                {
                    m_localPlayer = players[i];
                    m_localPlayer.SetPlayerInput(InputMapping.CreatePlayerInput(InputType.Keyboard1));
                    break;
                }
            }

            Debug.Assert(m_localPlayer != null);
            m_localPlayer.connection = peer.RemoteConnection;
            m_localPlayer.IsReady = true;
            m_localPlayer.needsFieldState = false;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        protected override void OnStateChanged(State oldState, State newState)
        {
            switch (newState)
            {
                case State.RoundStart:
                {
                    StartScreen(new BlockingScreen("Starting round..."));
                    break;
                }

                case State.Playing:
                {
                    Debug.Assert(m_localPlayer != null);
                    Debug.Assert(m_localPlayer.IsReady);
                    Debug.Assert(m_localPlayer.connection != null);

                    gameScreen = new GameScreen();
                    StartScreen(gameScreen);

                    m_localPlayer.ResetNetworkState();
                    m_localPlayer.IsReady = true;

                    gameScreen.AddDebugView(new NetworkTraceView(m_localPlayer.connection));
                    gameScreen.AddDebugView(new LocalPlayerView(m_localPlayer));
                    break;
                }

                case State.RoundEnd:
                {
                    SetPlayersReady(false);
                    StartScreen(new BlockingScreen("Waiting round results..."));
                    break;
                }

                case State.GameEnd:
                {
                    Stop();
                    break;
                }

                default:
                {
                    Debug.Fail("Unexpected state: " + newState);
                    break;
                }
            }
        }

        protected override void OnRoundEnded()
        {
            State state = GetState();
            if (state == State.Playing)
            {
                SetState(State.RoundEnd);
            }
        }

        protected override void RoundResultScreenAccepted(RoundResultScreen screen)
        {
            m_localPlayer.IsReady = true;
            StartScreen(new BlockingScreen("Waiting for server..."));
        }

        protected override void RoundResultScreenDismissed(RoundResultScreen screen)
        {
            Stop(ExitCode.StopClient);
        }

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
            }
        }
    }
}
