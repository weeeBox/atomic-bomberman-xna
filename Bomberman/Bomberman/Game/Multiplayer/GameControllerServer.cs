using System;
using System.Collections.Generic;
using BomberEngine;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Screens;
using Bomberman.Networking;
using Lidgren.Network;

namespace Bomberman.Game.Multiplayer
{
    public class GameControllerServer : GameControllerNetwork
    {
        private IDictionary<NetConnection, Player> networkPlayersLookup;
        private List<Player> networkPlayers;

        public GameControllerServer(GameSettings settings) :
            base(settings)
        {
        }

        protected override void OnStart()
        {
            base.OnStart();

            Application.SetWindowTitle("Server");

            game = new Game(MultiplayerMode.Server);

            networkPlayersLookup = new Dictionary<NetConnection, Player>();
            networkPlayers = new List<Player>();

            // local players are ready
            int playerIndex = 0;
            GameSettings.InputEntry[] entries = settings.inputEntries;
            for (; playerIndex < entries.Length; ++playerIndex)
            {
                Player player = new Player(entries[playerIndex].playerIndex);
                player.SetPlayerInput(entries[playerIndex].input);
                player.IsReady = true;
                game.AddPlayer(player);
            }


            // network players are not ready
            List<NetConnection> connections = GetServer().GetConnections();
            for (int i = 0; i < connections.Count; ++i)
            {
                Player player = new Player(playerIndex + i);
                player.SetPlayerInput(new PlayerNetworkInput());
                player.ResetNetworkState();
                game.AddPlayer(player);

                AddPlayerConnection(connections[i], player);
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
            networkPlayersLookup = null;
            networkPlayers = null;
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
                    for (int i = 0; i < networkPlayers.Count; ++i)
                    {
                        Player player = networkPlayers[i];
                        NetOutgoingMessage message = CreateMessage(PeerMessageId.RoundStart);

                        WriteReadyFlags(message);

                        if (player.needsFieldState)
                        {   
                            WriteFieldState(message, player);
                        }

                        NetConnection connection = player.connection;
                        Assert.IsTrue(connection != null);

                        SendMessage(message, connection);
                    }
                    break;
                }
                
                case State.Playing:
                {
                    NetOutgoingMessage payloadMessage = CreateMessage();
                    WritePlayingMessage(payloadMessage);

                    ++m_lastPacketId;

                    for (int i = 0; i < networkPlayers.Count; ++i)
                    {
                        Player player = networkPlayers[i];

                        NetOutgoingMessage message = CreateMessage(PeerMessageId.Playing);
                        message.Write(m_lastPacketId);              // packet to be acknowledged by client
                        message.Write(player.lastReceivedPackedId); // package acknowledged by server
                        message.Write(payloadMessage);

                        NetConnection connection = player.connection;
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

                    for (int i = 0; i < networkPlayers.Count; ++i)
                    {
                        Player player = networkPlayers[i];

                        NetOutgoingMessage message = CreateMessage(PeerMessageId.RoundEnd);

                        WriteReadyFlags(message);

                        if (player.needsRoundResults)
                        {
                            message.Write(payload);
                        }

                        NetConnection connection = player.connection;
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
            Player player = FindPlayer(msg.SenderConnection);
            player.IsReady = true;

            player.lastReceivedPackedId = msg.ReadInt32();
            player.lastAckPacketId = msg.ReadInt32();
            int actions = msg.ReadInt32((int)PlayerAction.Count);

            PlayerNetworkInput input = player.input as PlayerNetworkInput;
            Assert.IsTrue(input != null);

            input.SetNetActionBits(actions);
        }

        private void ReadRoundStartMessage(Peer peer, NetIncomingMessage msg)
        {
            State state = GetState();
            if (state == State.RoundStart)
            {
                Player player = FindPlayer(msg.SenderConnection);
                player.IsReady = msg.ReadBoolean();
                player.needsFieldState = msg.ReadBoolean();

                if (player.IsReady && AllPlayersAreReady())
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
                Player player = FindPlayer(msg.SenderConnection);

                player.IsReady = msg.ReadBoolean();
                player.needsRoundResults = msg.ReadBoolean();

                if (player.IsReady && AllPlayersAreReady())
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

            Player player = FindPlayer(connection);
            Assert.IsTrue(player != null);

            RemovePlayerConnection(connection);
            // TODO: notify gameplay

            if (CVars.g_startupMultiplayerMode.value != "server")
            {
                Application.sharedApplication.Stop();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Player connection

        private void AddPlayerConnection(NetConnection connection, Player player)
        {
            Assert.IsTrue(!networkPlayersLookup.ContainsKey(connection));
            networkPlayersLookup.Add(connection, player);

            Assert.IsTrue(player.connection == null);
            player.connection = connection;

            Assert.IsTrue(!networkPlayers.Contains(player));
            networkPlayers.Add(player);
        }

        private void RemovePlayerConnection(NetConnection connection)
        {
            Player player = FindPlayer(connection);
            Assert.IsTrue(player != null && player.connection == connection);
            player.connection = null;

            networkPlayersLookup.Remove(connection);

            Assert.IsTrue(networkPlayers.Contains(player));
            networkPlayers.Remove(player);
        }

        private Player TryFindPlayer(NetConnection connection)
        {
            Player player;
            if (networkPlayersLookup.TryGetValue(connection, out player))
            {
                return player;
            }

            return null;
        }

        private Player FindPlayer(NetConnection connection)
        {
            Player player = TryFindPlayer(connection);
            Assert.IsTrue(player != null);

            return player;
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
            for (int i = 0; i < networkPlayers.Count; ++i)
            {
                if (!networkPlayers[i].IsReady)
                {
                    return false;
                }
            }

            return true;
        }

        private void SetNetworkPlayersAreReady(bool ready)
        {
            for (int i = 0; i < networkPlayers.Count; ++i)
            {
                networkPlayers[i].IsReady = ready;
            }
        }

        #endregion
        
    }
}
