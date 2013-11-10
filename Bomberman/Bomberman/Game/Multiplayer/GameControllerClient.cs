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
        
        private ClientPacket[] m_sentPackets;

        private NetChannel m_channel;
        
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

            List<Player> localPlayers = new List<Player>();
            int index = 0;
            foreach (GameSettings.InputEntry entry in settings.inputEntries)
            {
                Player player = new Player(index++);
                player.SetPlayerInput(entry.input);
            }

            Client peer = GetClient();
            m_channel = new NetChannel(peer.RemoteConnection, localPlayers);
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
                    WriteRoundStartMessage(msg, m_channel);
                    SendMessage(msg);
                    break;
                }

                case State.Playing:
                {
                    NetOutgoingMessage msg = CreateMessage(PeerMessageId.Playing);
                    WritePlayingMessage(msg, m_channel);
                    SendMessage(msg);
                    break;
                }

                case State.RoundEnd:
                {
                    NetOutgoingMessage msg = CreateMessage(PeerMessageId.RoundEnd);
                    WriteRoundEndMessage(msg, m_channel);
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

        private void ReplayPlayerActions(NetChannel channel)
        {
            //float delta = Application.frameTime;
            //int oldMask = player.input.mask;

            //for (int id = player.acknowledgedSequence; id < m_lastPacketId; ++id)
            //{
            //    ClientPacket packet = GetPacket(id);
            //    Assert.IsTrue(!packet.replayed);
                
            //    int actions = packet.actions;
            //    player.input.Force(packet.actions);

            //    player.ReplayUpdate(delta);
            //    MarkReplayed(id);
            //}            
            //Assert.IsTrue(oldMask == player.input.mask);
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

            if (m_channel != null)
            {
                ReadReadyFlags(msg);
            }
            else
            {
                SkipReadyFlags(msg);
            }

            if (m_channel == null || m_channel.needsFieldState)
            {   
                ReadFieldState(peer, msg);
            }
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

            int acknowledgedSequence = m_channel.acknowledgedSequence;

            m_channel.incomingSequence = msg.ReadInt32();
            m_channel.acknowledgedSequence = msg.ReadInt32();

            if (acknowledgedSequence != m_channel.acknowledgedSequence)
            {
                ReadPlayingMessage(msg);
                ReplayPlayerActions(m_channel);
            }
            else
            {
                Log.d("Identical payload package");
            }
        }

        private void WritePlayingMessage(NetOutgoingMessage msg, NetChannel channel)
        {
            channel.outgoingSequence++;

            msg.Write(channel.outgoingSequence);    // packet to be acknowledged by server
            msg.Write(channel.incomingSequence);    // packet acknowledged by client

            List<Player> localPlayers = channel.players;
            for (int i = 0; i < localPlayers.Count; ++i)
            {
                WritePlayingMessage(msg, localPlayers[i]);
            }
        }

        private void WritePlayingMessage(NetOutgoingMessage msg, Player player)
        {
            msg.Write(player.input.mask);
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

        #region Packet history

        private void PushPacket(ref ClientPacket packet)
        {
            int index = packet.id % SENT_HISTORY_SIZE;
            m_sentPackets[index] = packet;
        }

        private ClientPacket GetPacket(int id)
        {
            int index = id % SENT_HISTORY_SIZE;
            Assert.IsTrue(m_sentPackets[index].id == id);
            return m_sentPackets[index];
        }

        private void MarkReplayed(int id)
        {
            int index = id % SENT_HISTORY_SIZE;
            if (m_sentPackets[index].id == id)
            {
                m_sentPackets[index].replayed = true;
            }
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
                // m_cordErrView.SetText("dpx: " + m_channel.errDx + "\ndpy: " + m_channel.errDy);
            }
        }
    }
}
