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

            game = new Game();
            game.AddPlayer(new Player(0));
            game.AddPlayer(new Player(1));
            game.AddPlayer(new Player(2));

            InitField(settings.scheme);

            gameScreen = new GameScreen();
            gameScreen.id = SCREEN_GAME;

            InitPlayers();
        }

        protected override void OnStart()
        {
            Console().RegisterCommands(gameCommands);
            StartScreen(gameScreen);

            switch (settings.mode)
            {
                case GameSettings.Mode.SinglePlayer:
                    break;
                case GameSettings.Mode.MultiplayerClient:
                    StartClient();
                    break;
                case GameSettings.Mode.MultiplayeServer:
                    StartServer();
                    break;
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

            PlayerKeyboardInput keyboardInput2 = new PlayerKeyboardInput();
            keyboardInput2.Map(KeyCode.Up, PlayerAction.Up);
            keyboardInput2.Map(KeyCode.Left, PlayerAction.Left);
            keyboardInput2.Map(KeyCode.Down, PlayerAction.Down);
            keyboardInput2.Map(KeyCode.Right, PlayerAction.Right);
            gameScreen.AddKeyListener(keyboardInput2);

            players[1].SetPlayerInput(keyboardInput2);

            PlayerGamePadInput gamePadInput = new PlayerGamePadInput(0);
            gamePadInput.Map(KeyCode.GP_A, PlayerAction.Bomb);
            gamePadInput.Map(KeyCode.GP_B, PlayerAction.Special);
            gameScreen.AddKeyListener(gamePadInput);
            gameScreen.AddUpdatable(gamePadInput);

            players[2].SetPlayerInput(gamePadInput);
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

            networkPeer = new ServerPeer(name, port);
            networkPeer.Start();
        }

        private void StartClient()
        {
            String name = CVars.sv_name.value;
            int port = CVars.sv_port.intValue;

            networkPeer = new ClientPeer(name, port);
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
