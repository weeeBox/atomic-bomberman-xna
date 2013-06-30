using System;
using System.Collections.Generic;
using BomberEngine.Core.Input;
using BomberEngine.Game;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Players.Input;
using Bomberman.Game.Screens;
using Bomberman.Networking;
using Lidgren.Network;

namespace Bomberman.Game.Multiplayer
{
    public class GameControllerClient : GameControllerNetwork, ClientListener
    {
        private Player localPlayer;

        public GameControllerClient(GameSettings settings)
            : base(settings)
        {
        }

        protected override void OnStart()
        {
            base.OnStart();

            Application.SetWindowTitle("Client");

            GetMultiplayerManager().SetClientListener(this);
            RequestFieldState();
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (game != null)
            {
                SendActions(localPlayer);
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Client listener

        public void OnMessageReceived(Client client, NetworkMessageId messageId, NetIncomingMessage message)
        {
            switch (messageId)
            {
                case NetworkMessageId.FieldState:
                    {
                        game = new Game();

                        SetupField(settings.scheme);

                        ReadFieldState(message);

                        gameScreen = new GameScreen();

                        localPlayer = null;
                        List<Player> players = game.GetPlayers().list;
                        for (int i = 0; i < players.Count; ++i)
                        {
                            if (players[i].input == null)
                            {
                                localPlayer = players[i];
                                break;
                            }
                        }

                        // input
                        PlayerKeyboardInput keyboardInput1 = new PlayerKeyboardInput();
                        keyboardInput1.Map(KeyCode.W, PlayerAction.Up);
                        keyboardInput1.Map(KeyCode.A, PlayerAction.Left);
                        keyboardInput1.Map(KeyCode.S, PlayerAction.Down);
                        keyboardInput1.Map(KeyCode.D, PlayerAction.Right);
                        keyboardInput1.Map(KeyCode.OemCloseBrackets, PlayerAction.Bomb);
                        keyboardInput1.Map(KeyCode.OemOpenBrackets, PlayerAction.Special);
                        gameScreen.AddKeyListener(keyboardInput1);

                        localPlayer.SetPlayerInput(keyboardInput1);
                        localPlayer.connection = client.GetServerConnection();

                        StartScreen(gameScreen);
                        break;
                    }

                case NetworkMessageId.PlayerPositions:
                    {
                        List<Player> players = game.GetPlayers().list;
                        ReadPlayersPositions(message, players);

                        break;
                    }
            }
        }

        public void OnConnectedToServer(Client client, NetConnection serverConnection)
        {
        }

        public void OnDisconnectedFromServer(Client client)
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Field state

        private void RequestFieldState()
        {
            SendMessage(NetworkMessageId.FieldState, NetDeliveryMethod.ReliableOrdered);
            StartConnectionScreen(OnRequestFieldStateCancelled, "Waiting for the server...");
        }

        private void OnRequestFieldStateCancelled()
        {
            throw new NotImplementedException();
        }

        #endregion

        private void SendActions(Player player)
        {
            NetOutgoingMessage msg = CreateMessage(NetworkMessageId.PlayerActions);
            WritePlayerActions(msg, player.input);
            SendMessage(msg, player.connection);
        }
    }
}
