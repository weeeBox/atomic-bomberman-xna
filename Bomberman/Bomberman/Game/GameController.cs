using System;
using System.Collections.Generic;
using BomberEngine.Game;
using Bomberman.Game.Screens;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Players.Input;
using Bomberman.Content;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Events;
using BomberEngine.Consoles;
using Bomberman.Networking;
using BomberEngine.Debugging;
using System.Net;
using BomberEngine.Core.IO;
using Bomberman.Game.Elements.Fields;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements;
using BomberEngine.Core;
using Bomberman.Multiplayer;
using Lidgren.Network;

namespace Bomberman.Game
{
    public class GameSettings
    {
        public enum Multiplayer
        {
            None,
            Server,
            Client
        }

        public Scheme scheme;
        public Multiplayer multiplayer;
        public ServerInfo serverInfo;

        public GameSettings(Scheme scheme)
        {
            this.scheme = scheme;
            multiplayer = Multiplayer.None;
        }
    }

    public class GameController : BombermanController, ClientListener, ServerListener
    {
        public enum ExitCode
        {
            StopServer,
            StopClient,
            Exit
        }

        public enum ClientState
        {
            Undefined,      // for the single game mode
            Created,        // initial state
            WaitFieldState, // wait until server responds with field state: players, powerup, etc
            Active,         // game in progress
        }

        public enum ServerState
        {
            Undefined,      // for the single game mode
            Created,        // initial state
            SendFieldState, // sends field state to clients
            Active,         // game in progress
        }

        private GameScreen gameScreen;
        private PauseScreen pauseScreen;

        private Game game;

        private GameSettings settings;

        private ClientState clientState;
        private ServerState serverState;

        private CCommand[] gameCommands = 
        {
            new Cmd_infect(),
            new Cmd_add(),
            new Cmd_map(),
            new Cmd_map_restart(),
        };

        public GameController(GameSettings settings)
        {
            this.settings = settings;
        }

        protected override void OnStart()
        {
            GetConsole().RegisterCommands(gameCommands);

            clientState = ClientState.Undefined;
            serverState = ServerState.Undefined;

            switch (settings.multiplayer)
            {
                case GameSettings.Multiplayer.None:
                {
                    game = new Game();
                    game.AddPlayer(new Player(0));

                    LoadField(settings.scheme);

                    gameScreen = new GameScreen();

                    InitPlayers();

                    StartScreen(gameScreen);
                    break;
                }
                case GameSettings.Multiplayer.Client:
                {
                    Application.SetWindowTitle("Client");

                    GetMultiplayerManager().SetClientListener(this);
                    clientState = ClientState.Created;
                    RequestFieldState();
                    
                    break;
                }
                case GameSettings.Multiplayer.Server:
                {
                    Application.SetWindowTitle("Server");

                    GetMultiplayerManager().SetServerListener(this);
                    serverState = ServerState.Created;

                    game = new Game();
                    game.AddPlayer(new Player(0));

                    LoadField(settings.scheme);

                    gameScreen = new GameScreen();

                    InitPlayers();

                    StartScreen(gameScreen);
                    break;
                }
                
                default:
                    Debug.Fail("Unexpected game mode: " + settings.multiplayer);
                    break;
            }
        }

        protected override void OnStop()
        {
            StopNetworkPeer();
            GetConsole().UnregisterCommands(gameCommands);
        }

        public void Stop(ExitCode exitCode, Object data = null)
        {
            Stop((int)exitCode, data);
        }

        private void InitPlayers()
        {
            List<Player> players = game.GetPlayers().list;

            PlayerKeyboardInput keyboardInput1 = new PlayerKeyboardInput();
            keyboardInput1.Map(KeyCode.W, PlayerAction.Up);
            keyboardInput1.Map(KeyCode.A, PlayerAction.Left);
            keyboardInput1.Map(KeyCode.S, PlayerAction.Down);
            keyboardInput1.Map(KeyCode.D, PlayerAction.Right);
            keyboardInput1.Map(KeyCode.OemCloseBrackets, PlayerAction.Bomb);
            keyboardInput1.Map(KeyCode.OemOpenBrackets, PlayerAction.Special);
            gameScreen.AddKeyListener(keyboardInput1);

            players[0].SetPlayerInput(keyboardInput1);
        }

        private void LoadField(Scheme scheme)
        {   
            game.LoadField(scheme); 
        }

        private void SetupField(Scheme scheme)
        {
            game.SetupField(scheme);
        }

        public override bool HandleEvent(Event evt)
        {
            if (base.HandleEvent(evt))
            {
                return true;
            }

            if (evt.code == Event.KEY)
            {
                KeyEvent keyEvent = evt as KeyEvent;
                if (keyEvent.state == KeyState.Pressed && OnKeyPressed(keyEvent.arg))
                {
                    return true;
                }
            }

            return false;
        }

        private bool OnKeyPressed(KeyEventArg e)
        {
            if (e.key == KeyCode.Escape)
            {
                Screen screen = CurrentScreen();
                if (screen is GameScreen)
                {
                    StartNextScreen(new PauseScreen(OnPauseScreenButtonPress));
                    return true;
                }

                if (screen is PauseScreen)
                {
                    screen.Finish();
                    return true;
                }
            }

            return false;
        }

        private void OnPauseScreenButtonPress(Button button)
        {
            PauseScreen.ButtonId buttonId = (PauseScreen.ButtonId)button.id;
            switch (buttonId)
            {
                case PauseScreen.ButtonId.Exit:
                    Stop(ExitCode.Exit);
                    break;
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Network messages

        private void RequestFieldState()
        {
            Debug.Assert(clientState == ClientState.Created);
            clientState = ClientState.WaitFieldState;

            SendMessage(NetworkMessageId.FieldState, NetDeliveryMethod.ReliableOrdered);
            StartConnectionScreen(OnRequestFieldStateCancelled, "Waiting for the server...");
        }

        private void OnRequestFieldStateCancelled()
        {
            throw new NotImplementedException();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Network

        private NetOutgoingMessage CreateMessage(NetworkMessageId messageId)
        {
            return GetMultiplayerManager().CreateMessage(messageId);
        }

        private void SendMessage(NetOutgoingMessage message, NetConnection recipient, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
        {
            GetMultiplayerManager().SendMessage(message, recipient, method);
        }

        private void SendMessage(NetworkMessageId messageId, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
        {
            GetMultiplayerManager().SendMessage(messageId, method);
        }

        private void SendMessage(NetOutgoingMessage message, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
        {
            GetMultiplayerManager().SendMessage(message, method);
        }

        private void SendMessage(NetworkMessageId messageId, NetConnection recipient, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
        {
            GetMultiplayerManager().SendMessage(messageId, recipient, method);
        }

        private void StopNetworkPeer()
        {
            GetMultiplayerManager().Stop();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Client listener

        public void OnMessageReceived(Client client, NetworkMessageId messageId, NetIncomingMessage message)
        {
            switch (messageId)
            {
                case NetworkMessageId.FieldState:
                {
                    game = new Game();
                    game.AddPlayer(new Player(0));

                    SetupField(settings.scheme);

                    ReadFieldState(message);

                    gameScreen = new GameScreen();

                    InitPlayers();

                    StartScreen(gameScreen);
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

        #region Server listener

        public void OnMessageReceived(Server server, NetworkMessageId messageId, NetIncomingMessage message)
        {
            switch (messageId)
            {
                case NetworkMessageId.FieldState:
                {
                    NetOutgoingMessage response = CreateMessage(NetworkMessageId.FieldState);
                    WriteFieldState(response);
                    SendMessage(response, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                    break;
                }
            }
        }

        public void OnClientConnected(Server server, string name, NetConnection connection)
        {
        }

        public void OnClientDisconnected(Server server, NetConnection connection)
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Messages

        private void WriteFieldState(NetOutgoingMessage response)
        {
            Field field = game.field;
            Debug.Assert(field != null);

            FieldCellSlot[] slots = field.GetCells().slots;
            for (int i = 0; i < slots.Length; ++i)
            {
                BrickCell brick = slots[i].GetBrick();
                if (brick != null)
                {
                    response.Write((byte)brick.powerup);
                }
            }
        }

        private void ReadFieldState(NetIncomingMessage response)
        {
            Field field = game.field;
            Debug.Assert(field != null);

            FieldCellSlot[] slots = field.GetCells().slots;
            for (int i = 0; i < slots.Length; ++i)
            {
                BrickCell brick = slots[i].GetBrick();
                if (brick != null)
                {
                    brick.powerup = response.ReadByte();
                }
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Connection screen

        private void StartConnectionScreen(ConnectionCancelCallback cancelCallback, String message)
        {
            NetworkConnectionScreen screen = new NetworkConnectionScreen(message);
            screen.cancelCallback = cancelCallback;
            StartNextScreen(screen);
        }

        private void HideConnectionScreen()
        {
            NetworkConnectionScreen screen = CurrentScreen() as NetworkConnectionScreen;
            if (screen != null)
            {
                screen.cancelCallback = null;
                screen.Finish();
            }
        }

        #endregion
    }
}
