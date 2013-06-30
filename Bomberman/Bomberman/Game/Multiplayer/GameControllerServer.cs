using System;
using System.Collections.Generic;
using BomberEngine.Debugging;
using BomberEngine.Game;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Players.Input;
using Bomberman.Game.Screens;
using Bomberman.Networking;
using Lidgren.Network;

namespace Bomberman.Game.Multiplayer
{
    public class GameControllerServer : GameControllerNetwork, IServerListener
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

            GetMultiplayerManager().SetServerListener(this);

            game = new Game();
            game.AddPlayer(new Player(0));

            networkPlayersLookup = new Dictionary<NetConnection, Player>();
            networkPlayers = new List<Player>();

            List<NetConnection> connections = GetServer().GetConnections();
            for (int i = 0; i < connections.Count; ++i)
            {
                Player player = new Player(i + i);
                player.SetPlayerInput(new PlayerNetworkInput());
                game.AddPlayer(player);

                AddPlayerConnection(connections[i], player);
            }

            LoadField(settings.scheme);

            gameScreen = new GameScreen();

            InitPlayers();

            StartScreen(gameScreen);
        }

        protected override void OnStop()
        {
            networkPlayersLookup = null;
            networkPlayers = null;
            base.OnStop();
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            SendServerPacket(delta);
        }

        private void SendServerPacket(float delta)
        {
            NetOutgoingMessage message = CreateMessage(NetworkMessageId.ServerPacket);
            WritePlayersPositions(message, game.GetPlayers().list);

            for (int i = 0; i < networkPlayers.Count; ++i)
            {
                NetConnection connection = networkPlayers[i].connection;
                Debug.Assert(connection != null);
                SendMessage(message, connection);
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Server listener

        public void OnMessageReceived(Server server, NetworkMessageId messageId, NetIncomingMessage message)
        {
            switch (messageId)
            {
                case NetworkMessageId.FieldState:
                    {
                        Player player = FindPlayer(message.SenderConnection);
                        Debug.Assert(player != null);

                        NetOutgoingMessage response = CreateMessage(NetworkMessageId.FieldState);
                        WriteFieldState(response, player);
                        SendMessage(response, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                        break;
                    }

                case NetworkMessageId.PlayerActions:
                    {
                        Player player = FindPlayer(message.SenderConnection);
                        Debug.Assert(player != null);

                        PlayerNetworkInput input = player.input as PlayerNetworkInput;
                        Debug.Assert(input != null);

                        ReadPlayerActions(message, input);
                        break;
                    }
            }
        }

        public void OnClientConnected(Server server, string name, NetConnection connection)
        {
            throw new NotImplementedException(); // clients can't join in progress game yet
        }

        public void OnClientDisconnected(Server server, NetConnection connection)
        {
            Player player = FindPlayer(connection);
            Debug.Assert(player != null);

            RemovePlayerConnection(connection);
            // TODO: notify gameplay
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

        private Player FindPlayer(NetConnection connection)
        {
            Player player;
            if (networkPlayersLookup.TryGetValue(connection, out player))
            {
                return player;
            }

            return null;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Game state

        private void FillGameState(GameState state)
        {
            List<Player> players = game.GetPlayers().list;
            state.playersCount = players.Count;
            for (int i = 0; i < players.Count; ++i)
            {
                state.players[i].SetFrom(players[i]);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        private Server GetServer()
        {
            return GetMultiplayerManager().GetServer();
        }

        #endregion
    }
}
