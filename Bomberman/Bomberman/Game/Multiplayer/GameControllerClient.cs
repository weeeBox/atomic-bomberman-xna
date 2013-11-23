using System;
using System.Collections.Generic;
using BomberEngine;
using Bomberman.Gameplay.Elements.Players;
using Bomberman.Gameplay.Screens;
using Bomberman.Networking;
using Lidgren.Network;

namespace Bomberman.Gameplay.Multiplayer
{
    public class GameControllerClient : GameControllerNetwork
    {
        private const int SENT_HISTORY_SIZE = 32;
        private const int SENT_HISTORY_MASK = SENT_HISTORY_SIZE-1;
        
        private ClientPacket[] m_sentPackets;

        private NetChannel m_channel;
        
        public GameControllerClient(Game game, GameSettings settings)
            : base(game, settings)
        {
            m_sentPackets = new ClientPacket[SENT_HISTORY_SIZE];
            for (int i = 0; i < SENT_HISTORY_SIZE; ++i)
            {
                m_sentPackets[i] = new ClientPacket();
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            Application.SetWindowTitle("Client");

            RegisterNotification(NetworkNotifications.ConnectedToServer,      ConnectedToServerNotification);
            RegisterNotification(NetworkNotifications.DisconnectedFromServer, DisconnectedFromServerNotification);

            StartScreen(new BlockingScreen("Waiting for server..."));

            List<Player> localPlayers = new List<Player>(game.GetPlayersList());
            Assert.IsTrue(localPlayers.Count > 0);

            #if DEBUG
            for (int i = 0; i < localPlayers.Count; ++i)
            {
                Assert.AreEqual(-1, localPlayers[i].index);
                Assert.IsTrue(localPlayers[i].input.IsLocal);
            }
            #endif

            Client peer = GetClient();
            m_channel = new NetChannel(peer.RemoteConnection, localPlayers);
            peer.RemoteConnection.Tag = m_channel;
        }

        #if UNIT_TESTING
        public void CreateNetChannel(NetConnection connection, List<Player>players)
        {
            m_channel = new NetChannel(connection, players);
        }
        #endif

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
                    SendRoundStartMessage();
                    break;
                }

                case State.Playing:
                {
                    SendPlayingSendMessage();
                    break;
                }

                case State.RoundEnd:
                {
                    SendRoundEndMessage();
                    break;
                }

                default:
                {
                    Debug.Fail("Unexpected state: " + state);
                    break;
                }
            }
        }

        internal void SendRoundStartMessage()
        {
            NetOutgoingMessage msg = CreateMessage(PeerMessageId.RoundStart);
            WriteRoundStartMessage(msg, m_channel);
            SendMessage(msg);
        }

        internal void SendPlayingSendMessage()
        {
            NetOutgoingMessage msg = CreateMessage(PeerMessageId.Playing);
            WritePlayingMessage(msg, m_channel);
            SendMessage(msg);

            // push packet
            ClientPacket packet = GetPacket(m_channel.outgoingSequence);
            packet.sequence = m_channel.outgoingSequence;
            packet.replayed = false;
            packet.frameTime = Application.frameTime;
            for (int i = 0; i < m_channel.players.Count; ++i)
            {
                packet.actions[i] = m_channel.players[i].input.mask;
            }
        }

        internal void SendRoundEndMessage()
        {
            NetOutgoingMessage msg = CreateMessage(PeerMessageId.RoundEnd);
            WriteRoundEndMessage(msg, m_channel);
            SendMessage(msg);
        }

        private void ReplayPlayerActions(NetChannel channel)
        {
            List<Player> players = channel.players;

            // reset actions
            ClientPacket packet = GetPacket(channel.acknowledgedSequence);
            for (int playerIndex = 0; playerIndex < channel.players.Count; ++playerIndex)
            {
                channel.players[playerIndex].input.Reset(packet.actions[playerIndex]);
            }

            // replay packets
            for (int seq = channel.acknowledgedSequence + 1; seq <= channel.outgoingSequence; ++seq)
            {
                packet = GetPacket(seq);
                for (int playerIndex = 0; playerIndex < channel.players.Count; ++playerIndex)
                {
                    channel.players[playerIndex].input.Force(packet.actions[playerIndex]);
                }

                for (int playerIndex = 0; playerIndex < channel.players.Count; ++playerIndex)
                {
                    channel.players[playerIndex].ReplayUpdate(packet.frameTime);
                }

                packet.replayed = true;
            }
        }

        internal ClientPacket GetPacket(int seq)
        {
            return m_sentPackets[seq & SENT_HISTORY_MASK];
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

            bool hasFieldState = msg.ReadBoolean();
            if (hasFieldState)
            {
                if (m_channel.needsFieldState)
                {
                    ReadFieldState(peer, msg);
                }
                else
                {
                    return; // we don't know the chunk size, so just skip it
                }
            }
            else // no field state
            {
                Assert.IsFalse(m_channel.needsFieldState);
            }

            ReadReadyFlags(msg);
        }

        private void WriteRoundStartMessage(NetBuffer buffer, NetChannel channel)
        {   
            buffer.Write(channel.IsReady);
            buffer.Write(channel.needsFieldState);
        }

        private void ReadPlayingMessage(Peer peer, NetIncomingMessage msg)
        {
            Assert.IsTrue(game != null);
            Assert.IsTrue(m_channel != null && m_channel.IsReady);

            SetState(State.Playing);
            
            ReadPlayingMessage(msg);
            ReplayPlayerActions(m_channel);
        }

        internal void WritePlayingMessage(NetBuffer msg, NetChannel channel)
        {
            List<Player> localPlayers = channel.players;
            for (int i = 0; i < localPlayers.Count; ++i)
            {   
                msg.Write(localPlayers[i].input.mask); // TODO: use bit packing
            }
        }

        private void ReadRoundEndMessage(Peer peer, NetIncomingMessage msg)
        {
            Assert.IsTrue(m_channel != null);

            SetState(State.RoundEnd);

            ReadReadyFlags(msg);

            if (m_channel.needsRoundResults)
            {
                ReadRoundResults(msg);
                m_channel.needsRoundResults = false;
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

        private void WriteRoundEndMessage(NetBuffer buffer, NetChannel channel)
        {
            buffer.Write(channel.IsReady);
            buffer.Write(channel.needsRoundResults);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Field state

        private void ReadFieldState(Peer peer, NetIncomingMessage msg)
        {
            SetupField(settings.scheme);
            ReadFieldState(msg);

            m_channel.IsReady = true;
            m_channel.needsFieldState = false;
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
                    Assert.IsTrue(m_channel != null);
                    Assert.IsTrue(m_channel.IsReady);
                    Assert.IsTrue(m_channel.connection != null);

                    gameScreen = new GameScreen();
                    StartScreen(gameScreen);

                    m_channel.Reset();
                    m_channel.IsReady = true;

                    gameScreen.AddDebugView(new NetworkTraceView(m_channel.connection));
                    gameScreen.AddDebugView(new LocalPlayerView(m_channel));
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
            m_channel.IsReady = true;
            StartScreen(new BlockingScreen("Waiting for server..."));
        }

        protected override void RoundResultScreenDismissed(RoundResultScreen screen)
        {
            Stop(ExitCode.StopClient);
        }

        //////////////////////////////////////////////////////////////////////////////

        private NetOutgoingMessage CreateMessage(PeerMessageId messageId)
        {
            return CreateMessage(messageId, m_channel);
        }

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
                m_roundTripView.SetText("Round trip: " + (int)(m_connection.AverageRoundtripTime * 1000));
                m_remoteTimeOffsetView.SetText("Remote time offset: " + (int)(m_connection.RemoteTimeOffset * 1000));

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
            private NetChannel m_channel;
            private TextView m_cordErrView;
            private TextView m_packetDiffView;

            public LocalPlayerView(NetChannel channel)
            {
                m_channel = channel;

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
                m_cordErrView.SetText("dpx: " + m_channel.players[0].errDx + "\ndpy: " + m_channel.players[0].errDy);
            }
        }
    }
}
