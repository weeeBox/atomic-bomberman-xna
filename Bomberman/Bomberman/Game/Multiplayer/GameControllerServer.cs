using System;
using System.Collections.Generic;
using BomberEngine.Debugging;
using BomberEngine.Game;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Players.Input;
using Bomberman.Game.Screens;
using Bomberman.Networking;
using Lidgren.Network;
using BomberEngine.Core;
using BomberEngine.Core.Events;
using BomberEngine.Consoles;
using Bomberman.Game.Elements.Fields;
using Bomberman.Multiplayer;

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

            int playerIndex = 0;
            GameSettings.InputEntry[] entries = settings.inputEntries;
            for (; playerIndex < entries.Length; ++playerIndex)
            {
                Player player = new Player(entries[playerIndex].playerIndex);
                player.SetPlayerInput(entries[playerIndex].input);
                player.IsReady = true;
                game.AddPlayer(player);
            }

            networkPlayersLookup = new Dictionary<NetConnection, Player>();
            networkPlayers = new List<Player>();

            List<NetConnection> connections = GetServer().GetConnections();
            for (int i = 0; i < connections.Count; ++i)
            {
                Player player = new Player(playerIndex + i);
                player.SetPlayerInput(new PlayerNetworkInput());
                player.IsReady = false;
                game.AddPlayer(player);

                AddPlayerConnection(connections[i], player);
            }

            LoadField(settings.scheme);

            gameScreen = new GameScreen();
            StartScreen(gameScreen);

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
            Application.CancelTimer(SendServerPacketCallback);
            Application.ScheduleTimer(SendServerPacketCallback, packetDelay, true);
        }

        private void ServerRateVarChangedCallback(Notification notification)
        {
            CVar var = notification.GetNotNullData<CVar>();
            SetPacketRate(var.intValue);
        }

        private void SendServerPacketCallback(Timer timer)
        {
            SendServerPacket();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Peer packet chunks

        private void SendServerPacket()
        {
            // TODO: refactor this code
            if (IsPlaying())
            {
                NetOutgoingMessage payloadMessage = CreateMessage();
                WriteServerIngameChunk(payloadMessage);

                for (int i = 0; i < networkPlayers.Count; ++i)
                {
                    Player player = networkPlayers[i];

                    NetOutgoingMessage message = CreateMessage(PeerMessageId.Playing);
                    message.Write(payloadMessage);

                    NetConnection connection = player.connection;
                    Debug.Assert(connection != null);

                    SendMessage(message, connection);
                }

                RecycleMessage(payloadMessage);
            }
            else if (IsStartingRound())
            {
                List<Player> players = game.GetPlayersList();
                for (int i = 0; i < players.Count; ++i)
                {
                    Player player = players[i];
                    if (!player.IsReady)
                    {
                        NetOutgoingMessage message = CreateMessage(PeerMessageId.RoundStart);
                        WriteFieldState(message, player);

                        NetConnection connection = player.connection;
                        Debug.Assert(connection != null);

                        SendMessage(message, connection);
                    }
                }
            }
            else if (IsEndingRound())
            {
                NetOutgoingMessage payloadMessage = CreateMessage();
                WriteServerIngameChunk(payloadMessage);

                for (int i = 0; i < networkPlayers.Count; ++i)
                {
                    Player player = networkPlayers[i];

                    NetOutgoingMessage message = CreateMessage(PeerMessageId.RoundEnd);
                    message.Write(payloadMessage);

                    NetConnection connection = player.connection;
                    Debug.Assert(connection != null);

                    SendMessage(message, connection);
                }

                RecycleMessage(payloadMessage);
            }
            else if (IsEndingGame())
            {
                NetOutgoingMessage payloadMessage = CreateMessage();
                WriteServerIngameChunk(payloadMessage);

                for (int i = 0; i < networkPlayers.Count; ++i)
                {
                    Player player = networkPlayers[i];

                    NetOutgoingMessage message = CreateMessage(PeerMessageId.GameEnd);
                    message.Write(payloadMessage);

                    NetConnection connection = player.connection;
                    Debug.Assert(connection != null);

                    SendMessage(message, connection);
                }

                RecycleMessage(payloadMessage);
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

                case PeerMessageId.GameEnd:
                    throw new NotImplementedException();
            }
        }

        private void ReadPlayingMessage(Peer peer, NetIncomingMessage msg)
        {
            Player player = FindPlayer(msg.SenderConnection);
            player.IsReady = true;

            ClientPacket packet = ReadClientPacket(msg);
            player.lastAckPacketId = packet.id;

            PlayerNetworkInput input = player.input as PlayerNetworkInput;
            Debug.Assert(input != null);

            input.SetNetActionBits(packet.actions);
        }

        private void ReadRoundStartMessage(Peer peer, NetIncomingMessage msg)
        {
            Player player = FindPlayer(msg.SenderConnection);
            player.IsReady = msg.ReadBoolean();

            bool allPlayersAreReady = true;
            for (int i = 0; i < networkPlayers.Count; ++i)
            {
                if (!networkPlayers[i].IsReady)
                {
                    allPlayersAreReady = false;
                    break;
                }
            }

            if (allPlayersAreReady)
            {
                Log.d("All players are ready");
                SetState(State.Playing);
            }
        }

        private void ReadRoundEndMessage(Peer peer, NetIncomingMessage msg)
        {
            Player player = FindPlayer(msg.SenderConnection);
            player.IsReady = msg.ReadBoolean();

            bool allPlayersAreReady = true;
            for (int i = 0; i < networkPlayers.Count; ++i)
            {
                if (!networkPlayers[i].IsReady)
                {
                    allPlayersAreReady = false;
                    break;
                }
            }

            if (allPlayersAreReady)
            {
                Log.d("All players are ready");
                throw new NotImplementedException();
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
            Debug.Assert(player != null);

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
            Debug.Assert(!networkPlayersLookup.ContainsKey(connection));
            networkPlayersLookup.Add(connection, player);

            Debug.Assert(player.connection == null);
            player.connection = connection;

            Debug.Assert(!networkPlayers.Contains(player));
            networkPlayers.Add(player);
        }

        private void RemovePlayerConnection(NetConnection connection)
        {
            Player player = FindPlayer(connection);
            Debug.Assert(player != null && player.connection == connection);
            player.connection = null;

            networkPlayersLookup.Remove(connection);

            Debug.Assert(networkPlayers.Contains(player));
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
            Debug.Assert(player != null);

            return player;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        protected override void OnRoundEnded()
        {
            SetState(State.RoundEnd);
            for (int i = 0; i < networkPlayers.Count; ++i)
            {
                networkPlayers[i].IsReady = false;
            }

            base.OnRoundEnded();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Game state

        private void FillGameState(GameStateSnapshot state)
        {   
            state.SetFrom(game.GetPlayers().list);
        }

        #endregion
        
    }
}
