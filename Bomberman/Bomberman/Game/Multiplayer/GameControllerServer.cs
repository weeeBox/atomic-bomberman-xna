using System;
using System.Collections.Generic;
using BomberEngine;
using Bomberman.Gameplay.Elements.Players;
using Bomberman.Gameplay.Screens;
using Bomberman.Networking;
using Lidgren.Network;

namespace Bomberman.Gameplay.Multiplayer
{
    public class GameControllerServer : GameControllerNetwork
    {
        private List<NetChannel> m_channels;

        public GameControllerServer(Game game, GameSettings settings) :
            base(game, settings)
        {
        }

        protected override void OnStart()
        {
            base.OnStart();

            Application.SetWindowTitle("Server");

            m_channels = new List<NetChannel>();

            // local players are ready
            int playerIndex = 0;
            List<Player> localPlayers = game.GetPlayersList();
            for (; playerIndex < localPlayers.Count; ++playerIndex)
            {
                Player player = localPlayers[playerIndex];
                player.IsReady = true;

                Assert.AreEqual(playerIndex, player.index);
                Assert.IsTrue(player.input.IsLocal);
            }

            Assert.IsTrue(playerIndex > 0); // should be some local players at the point

            // network players are not ready
            List<NetConnection> connections = GetServer().GetConnections();
            for (int i = 0; i < connections.Count; ++i)
            {
                Player player = new Player(playerIndex + i);
                player.SetPlayerInput(new PlayerNetworkInput());
                player.ResetNetworkState();
                game.AddPlayer(player);

                AddChannel(connections[i], new NetChannel(connections[i], player));
            }

            LoadField(settings.scheme);

            GetConsole().TryExecuteCommand("exec game.cfg");

            SetPacketRate(CVars.sv_packetRate.intValue);

            RegisterNotification(Notifications.ConsoleVariableChanged, ServerRateVarChangedCallback);
            RegisterNotification(NetworkNotifications.ClientConnected, ClientConnectedNotification);
            RegisterNotification(NetworkNotifications.ClientDisconnected, ClientDisconnectedNotification);
        }

        protected override void OnStop()
        {
            m_channels = null;
            Application.CancelAllTimers(this);

            base.OnStop();
        }

        private void SetPacketRate(int packetRate)
        {
            float packetDelay = 1.0f / packetRate;
            Application.CancelTimer(SendServerPacket);
            Application.ScheduleTimer(SendServerPacket, packetDelay, true);
        }

        private void ServerRateVarChangedCallback(Notification notification)
        {
            CVar var = notification.GetNotNullData<CVar>();
            SetPacketRate(var.intValue);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Peer packet chunks

        private void SendServerPacket()
        {
            State state = GetState();
            switch (state)
            {
                case State.RoundStart:
                {
                    for (int i = 0; i < m_channels.Count; ++i)
                    {
                        NetChannel channel = m_channels[i];
                        NetOutgoingMessage msg = CreateMessage(PeerMessageId.RoundStart, channel);

                        // field state
                        msg.Write(channel.needsFieldState);
                        if (channel.needsFieldState)
                        {   
                            WriteFieldState(msg, channel);
                        }

                        // ready flags
                        WriteReadyFlags(msg);

                        NetConnection connection = channel.connection;
                        Assert.IsTrue(connection != null);

                        SendMessage(msg, connection);
                    }
                    break;
                }
                
                case State.Playing:
                {
                    NetOutgoingMessage payloadMessage = CreateMessage();
                    WritePlayingMessage(payloadMessage);

                    for (int i = 0; i < m_channels.Count; ++i)
                    {
                        NetChannel channel = m_channels[i];

                        NetOutgoingMessage message = CreateMessage(PeerMessageId.Playing, channel);
                        message.Write(payloadMessage);

                        NetConnection connection = channel.connection;
                        Assert.IsTrue(connection != null);

                        SendMessage(message, connection);
                    }

                    RecycleMessage(payloadMessage);
                    break;
                }

                case State.RoundEnd:
                {
                    NetOutgoingMessage payload = CreateMessage();
                    WriteRoundResults(payload);

                    for (int i = 0; i < m_channels.Count; ++i)
                    {
                        NetChannel channel = m_channels[i];

                        NetOutgoingMessage message = CreateMessage(PeerMessageId.RoundEnd, channel);

                        WriteReadyFlags(message);

                        if (channel.needsRoundResults)
                        {
                            message.Write(payload);
                        }

                        NetConnection connection = channel.connection;
                        Assert.IsTrue(connection != null);

                        SendMessage(message, connection);
                    }

                    RecycleMessage(payload);
                    break;
                }

                default:
                {
                    Debug.Fail("Unexpected state: " + state);
                    break;
                }
            }
        }

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
            }
        }

        private void ReadPlayingMessage(Peer peer, NetIncomingMessage msg)
        {
            NetChannel channel = GetChannel(msg.SenderConnection);
            channel.IsReady = true;

            ReadPlayingMessage(msg, channel);
        }

        internal void ReadPlayingMessage(NetBuffer msg, NetChannel channel)
        {
            List<Player> players = channel.players;
            for (int i = 0; i < players.Count; ++i)
            {
                int actions = msg.ReadInt32(); // TODO: use bit packing

                PlayerNetworkInput input = ClassUtils.Cast<PlayerNetworkInput>(players[i].input);
                input.SetNetActionBits(actions);
            }
        }

        private void ReadRoundStartMessage(Peer peer, NetIncomingMessage msg)
        {
            State state = GetState();
            if (state == State.RoundStart)
            {
                NetChannel channel = GetChannel(msg.SenderConnection);
                channel.IsReady = msg.ReadBoolean();
                channel.needsFieldState = msg.ReadBoolean();

                if (channel.IsReady && AllPlayersAreReady())
                {
                    Log.d("Clients are ready to play");
                    SetState(State.Playing);
                }
            }
            else
            {
                Log.d("Ignore 'RoundStart' message");
            }
        }

        private void ReadRoundEndMessage(Peer peer, NetIncomingMessage msg)
        {
            State state = GetState();
            if (state == State.RoundEnd)
            {
                NetChannel channel = GetChannel(msg.SenderConnection);

                channel.IsReady = msg.ReadBoolean();
                channel.needsRoundResults = msg.ReadBoolean();

                if (channel.IsReady && AllPlayersAreReady())
                {
                    if (game.IsGameEnded)
                    {
                        SetState(State.GameEnd);
                    }
                    else
                    {                    
                        Log.d("Clients are ready for the next round");
                        game.StartNextRound();

                        SetState(State.RoundStart);
                    }
                }
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Notifications

        private void ClientConnectedNotification(Notification notification)
        {
            NetConnection connection = notification.GetData<NetConnection>();
            String name = notification.GetData2<String>();

            throw new NotImplementedException(); // clients can't join in progress game yet
        }

        private void ClientDisconnectedNotification(Notification notification)
        {
            NetConnection connection = notification.GetData<NetConnection>();
            Assert.IsNotNull(connection);

            RemoveChannel(connection);
            // TODO: notify gameplay

            if (CVars.g_startupMultiplayerMode.value != "server")
            {
                Application.sharedApplication.Stop();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Channels

        private void AddChannel(NetConnection connection, NetChannel channel)
        {
            Assert.IsNull(connection.Tag);
            connection.Tag = channel;

            Assert.IsFalse(m_channels.Contains(channel));
            m_channels.Add(channel);
        }

        private void RemoveChannel(NetConnection connection)
        {
            NetChannel channel = GetChannel(connection);

            bool removed = m_channels.Remove(channel);
            Assert.IsTrue(removed);
        }

        private NetChannel GetChannel(NetConnection connection)
        {
            return ClassUtils.Cast<NetChannel>(connection.Tag);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        protected override void OnRoundEnded()
        {
            Assert.IsTrue(GetState() == State.Playing);

            SetPlayersReady(false);
            SetState(State.RoundEnd);
        }

        protected override void OnGameEnded()
        {
            Assert.IsTrue(GetState() == State.Playing);

            SetPlayersReady(false);
            SetState(State.RoundEnd);
        }

        protected override void OnRoundRestarted()
        {   
        }

        protected override void RoundResultScreenAccepted(RoundResultScreen screen)
        {
            List<Player> players = game.GetPlayersList();
            for (int i = 0; i < players.Count; ++i)
            {
                if (!players[i].IsNetworkPlayer)
                {
                    players[i].IsReady = true;
                    break;
                }
            }

            StartScreen(new BlockingScreen("Waiting for clients..."));
        }

        protected override void OnStateChanged(State oldState, State newState)
        {
            switch (newState)
            {
                case State.RoundStart:
                {
                    Assert.IsTrue(oldState == State.RoundEnd || oldState == State.Undefined,
                        "Unexpected old state: " + oldState);

                    StartScreen(new BlockingScreen("Waiting for clients..."));

                    if (game != null)
                    {
                        List<Player> players = game.GetPlayersList();
                        for (int i = 0; i < players.Count; ++i)
                        {
                            Player p = players[i];
                            p.IsReady = !p.IsNetworkPlayer;
                        }
                    }

                    break;
                }

                case State.Playing:
                {
                    Assert.IsTrue(oldState == State.RoundStart, "Unexpected old state: " + oldState);
                    Assert.IsTrue(!(CurrentScreen() is GameScreen));

                    gameScreen = new GameScreen();
                    StartScreen(gameScreen);
                    break;
                }

                case State.RoundEnd:
                {
                    Assert.IsTrue(oldState == State.Playing, "Unexpected old state: " + oldState);

                    Restart();
                    if (game.IsGameEnded)
                    {
                        StartGameResultScreen();
                    }
                    else
                    {
                        StartRoundResultScreen();
                    }
                    break;
                }

                case State.GameEnd:
                {
                    Stop();
                    break;
                }
                  
                default:
                {
                    Debug.Fail("Unexpected old state: " + newState);
                    break;
                }
            }
        }
        
        //////////////////////////////////////////////////////////////////////////////

        #region Game state

        private void FillGameState(GameStateSnapshot state)
        {   
            state.SetFrom(game.GetPlayers().list);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        private bool NetworkPlayersAreReady()
        {
            for (int i = 0; i < m_channels.Count; ++i)
            {
                if (!m_channels[i].IsReady)
                {
                    return false;
                }
            }

            return true;
        }

        private void SetNetworkPlayersAreReady(bool ready)
        {
            for (int i = 0; i < m_channels.Count; ++i)
            {
                m_channels[i].IsReady = ready;
            }
        }

        #endregion
        
    }
}
