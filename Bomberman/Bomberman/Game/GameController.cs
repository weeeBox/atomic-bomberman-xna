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
using Bomberman.Network;
using BomberEngine.Debugging;
using System.Net;

namespace Bomberman.Game
{
    public class GameSettings
    {
        public enum Mode
        {
            SinglePlayer,
            MultiplayeServer,
            MultiplayerClient
        }

        public String scheme;
        public Mode mode;
        public ServerInfo serverInfo;

        public GameSettings(String scheme)
        {
            this.scheme = scheme;
            mode = Mode.SinglePlayer;
        }
    }

    public class GameController : Controller
    {
        private const int SCREEN_GAME = 1;
        private const int SCREEN_PAUSE = 2;

        private GameScreen gameScreen;
        private Game game;

        private GameSettings settings;
        private NetworkPeer networkPeer;

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
            Console().RegisterCommands(gameCommands);
            StartScreen(gameScreen);

            switch (settings.mode)
            {
                case GameSettings.Mode.SinglePlayer:
                {
                    game = new Game();
                    game.AddPlayer(new Player(0));

                    InitField(settings.scheme);

                    gameScreen = new GameScreen();
                    gameScreen.id = SCREEN_GAME;

                    InitPlayers();
                    break;
                }
                case GameSettings.Mode.MultiplayerClient:
                {
                    game = new Game();
                    game.AddPlayer(new Player(0));

                    gameScreen = new GameScreen();
                    gameScreen.id = SCREEN_GAME;

                    InitPlayers();

                    StartClient();
                    break;
                }
                case GameSettings.Mode.MultiplayeServer:
                {
                    game = new Game();
                    game.AddPlayer(new Player(0));

                    InitField(settings.scheme);

                    gameScreen = new GameScreen();
                    gameScreen.id = SCREEN_GAME;

                    InitPlayers();

                    StartServer();
                    break;
                }

                default:
                    Debug.Fail("Unexpected game mode: " + settings.mode);
                    break;
            }
        }

        protected override void OnStop()
        {
            StopNetworkPeer();
            Console().UnregisterCommands(gameCommands);
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

        private void InitField(String schemeName)
        {
            Scheme scheme = Application.Assets().LoadAsset<Scheme>("Content\\maps\\" + schemeName + ".sch");
            game.LoadField(scheme); 
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
                if (screen.id == SCREEN_GAME)
                {
                    PauseScreen pauseScreen = new PauseScreen(OnPauseScreenButtonPress);
                    pauseScreen.id = SCREEN_PAUSE;
                    StartNextScreen(pauseScreen);
                    return true;
                }

                if (screen.id == SCREEN_PAUSE)
                {
                    screen.Finish();
                    return true;
                }
            }

            return false;
        }

        private void OnPauseScreenButtonPress(Button button)
        {
            switch (button.id)
            {
                case PauseScreen.BUTTON_RESUME:
                    CurrentScreen().Finish();
                    break;

                case PauseScreen.BUTTON_EXIT:
                    Stop();
                    break;
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
            String name = CVars.sv_name.value;
            int port = CVars.sv_port.intValue;

            networkPeer = new NetworkServer(name, port);
            networkPeer.Start();
        }

        private void StartClient()
        {
            ServerInfo serverInfo = settings.serverInfo;
            Debug.Assert(serverInfo != null);

            String name = CVars.sv_name.value;
            IPEndPoint endPoint = serverInfo.endPoint;

            networkPeer = new NetworkClient(name, endPoint);
            networkPeer.Start();

            Log.d("Started network peer");
        }

        private void StopNetworkPeer()
        {
            if (networkPeer != null)
            {
                networkPeer.Stop();
                networkPeer = null;

                Log.d("Stopped network peer");
            }
        }

        #endregion
    }
}
