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

    public class GameController : Controller
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
        private Peer networkPeer;

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

                    clientState = ClientState.Created;

                    game = new Game();
                    game.AddPlayer(new Player(0));

                    SetupField(settings.scheme);

                    gameScreen = new GameScreen();

                    InitPlayers();

                    StartScreen(gameScreen);
                    break;
                }
                case GameSettings.Multiplayer.Server:
                {
                    Application.SetWindowTitle("Server");

                    serverState = ServerState.Created;

                    game = new Game();
                    game.AddPlayer(new Player(0));

                    SetupField(settings.scheme);

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

        private void OnLobbyScreenButtonPressed(Button button)
        {
            MultiplayerLobbyScreen.ButtonId buttonId = (MultiplayerLobbyScreen.ButtonId)button.id;
            switch (buttonId)
            {
                case MultiplayerLobbyScreen.ButtonId.Start:
                {
                    game = new Game();
                    game.AddPlayer(new Player(0));

                    LoadField(settings.scheme);

                    gameScreen = new GameScreen();

                    InitPlayers();

                    StartScreen(gameScreen);
                    break;
                }

                case MultiplayerLobbyScreen.ButtonId.Back:
                {
                    switch (settings.multiplayer)
                    {
                        case GameSettings.Multiplayer.Server:
                            Stop(ExitCode.StopServer);
                            break;
                        case GameSettings.Multiplayer.Client:
                            Stop(ExitCode.StopClient);
                            break;
                        case GameSettings.Multiplayer.None:
                            Stop(ExitCode.Exit);
                            break;
                        default:
                            Debug.Fail("Unexpected exit code: " + exitCode);
                            break;
                    }
                    break;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Updatable

        public override void Update(float delta)
        {
            base.Update(delta);

            if (networkPeer != null)
            {
                networkPeer.Update(delta);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Net peer

        private void StartServer()
        {
            //String name = CVars.sv_name.value;
            //int port = CVars.sv_port.intValue;

            //Server server = new Server(name, port);
            //server.listener = this;
            //networkPeer = server;
            
            //networkPeer.Start();

            //Log.d("Started network server");
        }

        private void StartClient()
        {
            //ServerInfo serverInfo = settings.serverInfo;
            //Debug.Assert(serverInfo != null);

            //String name = CVars.sv_name.value;
            //IPEndPoint endPoint = serverInfo.endPoint;

            //Client client = new Client(name, endPoint);
            //client.listener = this;
            //networkPeer = client;
            
            //networkPeer.Start();

            //Log.d("Started network client");
        }

        private void StopNetworkPeer()
        {
            //if (networkPeer != null)
            //{
            //    networkPeer.Stop();
            //    networkPeer = null;

            //    Log.d("Stopped network peer");
            //}
        }

        #endregion

        //public void OnMessageReceived(Client client, Connection connection, NetworkMessageId message)
        //{
        //    switch (message)
        //    {
        //        case NetworkMessageId.FieldState:
        //        {
        //            BitReadBuffer buffer = connection.ReadBuffer;
        //            ReadFieldState(buffer);
        //            break;
        //        }
        //    }
        //}

        //public void OnClientConnected(Server server, Connection connection)
        //{
        //    BitWriteBuffer buffer = connection.WriteBuffer;
        //    WriteFieldState(buffer);
        //    connection.SendMessage(NetworkMessageId.FieldState, buffer);

        //    MultiplayerLobbyScreen lobbyScreen = CurrentScreen() as MultiplayerLobbyScreen;
        //    Debug.Assert(lobbyScreen != null);

        //    lobbyScreen.AddPlayer(connection);
        //}

        //public void OnClientDisconnected(Server server, Connection connection)
        //{
        //}

        //public void OnMessageReceived(Server server, Connection connection, NetworkMessageId message)
        //{   
        //}

        //public void OnConnectedToServer(Client client, Connection serverConnection)
        //{   
        //}

        //public void OnDisconnectedFromServer(Client client)
        //{
        //}

        //public void WriteDiscoveryResponse(BitWriteBuffer buffer)
        //{
        //    Field field = game.field;

        //    FieldCellArray cells = field.GetCells();
        //    int width = cells.GetWidth();
        //    int height = cells.GetHeight();

        //    buffer.Write(width);
        //    buffer.Write(height);

        //    FieldCellSlot[] slots = cells.slots;
        //    for (int i = 0; i < slots.Length; ++i)
        //    {
        //        FieldCell staticCell = slots[i].staticCell;
        //        if (staticCell != null)
        //        {
        //            if (staticCell.IsSolid())
        //            {
        //                buffer.Write(BLOCK_SOLID);
        //            }
        //            else if (staticCell.IsBrick())
        //            {   
        //                buffer.Write(BLOCK_BRICK);
        //            }
        //        }
        //        else
        //        {
        //            buffer.Write(BLOCK_EMPTY);
        //        }
        //    }
        //}

        //private const byte BLOCK_EMPTY = 0;
        //private const byte BLOCK_SOLID = 1;
        //private const byte BLOCK_BRICK = 2;

        //private const byte NO_POWERUP = (byte)0xff;

        //private void WriteFieldState(BitWriteBuffer buffer)
        //{
        //    Field field = game.field;

        //    FieldCellArray cells = field.GetCells();
        //    int width = cells.GetWidth();
        //    int height = cells.GetHeight();

        //    buffer.Write(width);
        //    buffer.Write(height);

        //    FieldCellSlot[] slots = cells.slots;
        //    for (int i = 0; i < slots.Length; ++i)
        //    {
        //        FieldCell staticCell = slots[i].staticCell;
        //        if (staticCell != null)
        //        {
        //            if (staticCell.IsSolid())
        //            {
        //                buffer.Write(BLOCK_SOLID);
        //            }
        //            else if (staticCell.IsBrick())
        //            {
        //                BrickCell brick = staticCell.AsBrick();
        //                byte powerup = brick.powerup != Powerups.None ? (byte)brick.powerup : NO_POWERUP;

        //                buffer.Write(BLOCK_BRICK);
        //                buffer.Write(powerup);
        //            }
        //        }
        //        else
        //        {
        //            buffer.Write(BLOCK_EMPTY);
        //        }
        //    }
        //}

        //private void ReadFieldState(BitReadBuffer buffer)
        //{
        //    Field field = game.field;

        //    int width = buffer.ReadInt32();
        //    int height = buffer.ReadInt32();

        //    field.Init(width, height);

        //    for (int y = 0; y < height; ++y)
        //    {
        //        for (int x = 0; x < width; ++x)
        //        {
        //            byte blockType = buffer.ReadByte();
        //            if (blockType == BLOCK_EMPTY)
        //            {
        //                // nothing
        //            }
        //            else if (blockType == BLOCK_BRICK)
        //            {
        //                BrickCell brick = new BrickCell(x, y);

        //                byte powerup = buffer.ReadByte();
        //                if (powerup != NO_POWERUP)
        //                {
        //                    brick.powerup = powerup;
        //                }

        //                field.AddCell(brick);
        //            }
        //            else if (blockType == BLOCK_SOLID)
        //            {
        //                SolidCell solid = new SolidCell(x, y);
        //                field.AddCell(solid);
        //            }
        //        }
        //    }

        //    gameScreen = new GameScreen();

        //    InitPlayers();

        //    StartScreen(gameScreen);
        //}
    }
}
