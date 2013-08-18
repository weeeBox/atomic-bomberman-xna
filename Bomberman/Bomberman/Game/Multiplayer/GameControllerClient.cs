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
            if (IsIngame())
            {
                NetOutgoingMessage msg = CreateMessage(PeerMessageId.Playing);
                WritePlayingMessage(msg);
                SendMessage(msg);
            }
            else if (IsWaitingRoundRestart() || isWaitingRoundResult())
            {
                NetOutgoingMessage msg = CreateMessage(PeerMessageId.RoundEnd);
                WriteRoundEndMessage(msg);
                SendMessage(msg);
            }
            else if (IsWaitingGameStart() || IsWaitingIngame())
            {
                NetOutgoingMessage msg = CreateMessage(PeerMessageId.RoundStart);
                WriteRoundStartMessage(msg);
                SendMessage(msg);
            }
        }

        private void ReplayPlayerActions(Player player)
        {
            //float delta = Application.frameTime;
            //player.input.Reset();

            //for (int id = player.lastAckPacketId; id <= m_lastSentPacketId; ++id)
            //{
            //    ClientPacket packet = GetPacket(id);
            //    int actions = packet.actions;
            //    int actionsCount = (int)PlayerAction.Count;
            //    for (int i = 0; i < actionsCount; ++i)
            //    {
            //        player.input.SetActionPressed(i, (actions & (1 << i)) != 0);
            //    }

            //    player.UpdateMoving(delta);
            //}

            throw new NotImplementedException();
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
            if (IsWaitingGameStart())
            {
                OnFieldStateReceived(peer, msg);
                SetState(State.WaitIngame);
            }
        }

        private void WriteRoundStartMessage(NetBuffer buffer)
        {
            bool isReady = m_localPlayer != null && m_localPlayer.IsReady;
            buffer.Write(isReady);
        }

        private void ReadPlayingMessage(Peer peer, NetIncomingMessage msg)
        {
            Debug.Assert(game != null);
            Debug.Assert(m_localPlayer != null && m_localPlayer.IsReady);

            SetState(State.Ingame);

            ReadPlayingMessage(msg);

            if (!CVars.sv_dumbClient.boolValue)
            {
                ReplayPlayerActions(m_localPlayer);
            }
        }

        private void WritePlayingMessage(NetOutgoingMessage msg)
        {
            Player player = m_localPlayer;
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
            packet.id = player.lastSentPacketId;
            packet.lastAckServerPacketId = player.lastAckPacketId;
            packet.actions = actions;

            WriteClientPacket(msg, ref packet);

            PushPacket(ref packet);
        }

        private void ReadRoundEndMessage(Peer peer, NetIncomingMessage msg)
        {
            ReadRoundEndMessage(msg);
            SetState(State.WaitRoundRestart);
        }

        private void WriteRoundEndMessage(NetBuffer buffer)
        {
            bool isReady = m_localPlayer.IsReady;
            buffer.Write(isReady);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Field state

        private void OnFieldStateReceived(Peer peer, NetIncomingMessage message)
        {
            game = new Game(MultiplayerMode.Client);

            SetupField(settings.scheme);
            ReadFieldState(message);

            bool shouldBeReady = m_localPlayer == null;
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
            m_localPlayer.IsReady = shouldBeReady;
            m_localPlayer.connection = peer.RemoteConnection;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        protected override void OnStateChanged(State oldState, State newState)
        {
            switch (newState)
            {
                case State.WaitGameStart:
                {
                    Debug.Assert(oldState == State.Undefined, "Unexpected old state: " + oldState);
                    StartScreen(new BlockingScreen("Waiting round start..."));
                    break;
                }

                case State.WaitIngame:
                {   
                    break;
                }

                case State.WaitRoundResult:
                {
                    Debug.Assert(oldState == State.Ingame, "Unexpected old state: " + oldState);

                    m_localPlayer.IsReady = false;
                    StartScreen(new BlockingScreen("Waiting round result..."));
                    break;
                }

                case State.WaitRoundRestart:
                {
                    Debug.Assert(oldState == State.WaitRoundResult || oldState == State.Ingame, "Unexpected old state: " + oldState);

                    m_localPlayer.IsReady = false;
                    StartRoundResultScreen();

                    break;
                }

                case State.Ingame:
                {
                    Debug.Assert(m_localPlayer != null);
                    Debug.Assert(m_localPlayer.IsReady);

                    gameScreen = new GameScreen();
                    StartScreen(gameScreen);

                    Debug.Assert(m_localPlayer != null);
                    Debug.Assert(m_localPlayer.connection != null);

                    gameScreen.AddDebugView(new NetworkTraceView(m_localPlayer.connection));
                    gameScreen.AddDebugView(new LocalPlayerView(m_localPlayer));
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
            if (IsIngame())
            {
                SetState(State.WaitRoundResult);
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
                m_packetDiffView.SetText("packet diff: " + m_player.networkPackageDiff);
            }
        }
    }
}
