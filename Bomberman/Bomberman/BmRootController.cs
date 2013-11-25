using System;
using Assets;
using BomberEngine;
using Bomberman.Content;
using Bomberman.Gameplay;
using Bomberman.Menu;
using Bomberman.Multiplayer;
using Bomberman.Networking;
using Microsoft.Xna.Framework.Content;
using Bomberman.Gameplay.Elements.Players;

namespace Bomberman
{
    public class BmRootController : RootController
    {
        private MenuController m_menuController;
        private NetworkManager m_networkManager;

        private Game m_game;
        private InputMapping m_mapping;

        public BmRootController(ContentManager contentManager)
            : base(contentManager)
        {
            m_networkManager = new NetworkManager();
            m_mapping = new InputMapping();
            m_game = new Game();
        }

        #if UNIT_TESTING
        protected BmRootController()
        {
        }
        #endif

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        protected override void OnStart()
        {
            BmAssetManager manager = (BmAssetManager)Application.Assets();
            manager.AddPackToLoad(A.Packs.all);
            manager.LoadImmediately();

            StartMainMenuController();

            String mode = CVars.g_startupMultiplayerMode.value;
            if (mode != null)
            {
                if (mode.Equals("client"))
                {
                    m_menuController.Stop((int)MenuController.ExitCode.DebugClientStart);
                }
                else if (mode.Equals("server"))
                {
                    m_menuController.Stop((int)MenuController.ExitCode.DebugServerStart);
                }
            }
        }

        protected override void OnStop()
        {
            m_mapping.Dispose();
            base.OnStop();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Child controllers

        protected override void OnControllerStop(Controller controller)
        {
            if (controller is MenuController)
            {
                if (controller.IsExiting)
                {   
                    return;
                }

                MenuController.ExitCode exitCode = (MenuController.ExitCode)controller.exitCode;
                switch (exitCode)
                {
                    case MenuController.ExitCode.SingleStart:
                    {
                        StartController(new GameLobbyController());
                        break;
                    }

                    case MenuController.ExitCode.MultiplayerStart:
                    {
                        StartController(new MultiplayerController());
                        break;
                    }

                    case MenuController.ExitCode.DebugClientStart:
                    {
                        StartController(new DebugMultiplayerController(DebugMultiplayerController.Mode.Client));
                        break;
                    }

                    case MenuController.ExitCode.DebugServerStart:
                    {
                        StartController(new DebugMultiplayerController(DebugMultiplayerController.Mode.Server));
                        break;
                    }

                    case MenuController.ExitCode.Exit:
                    {
                        Application.sharedApplication.Stop();
                        break;
                    }
                }
            }
            else if (controller is MultiplayerController)
            {
                if (controller.IsExiting)
                {
                    StartMainMenuController();
                    return;
                }

                MultiplayerController.ExitCode exitCode = (MultiplayerController.ExitCode)controller.exitCode;
                switch (exitCode)
                {
                    case MultiplayerController.ExitCode.Create:
                        break;

                    case MultiplayerController.ExitCode.Join:
                        break;
                }
            }
            else if (controller is GameController)
            {
                StartMainMenuController();
            }
            else if (controller is GameLobbyController)
            {
                if (controller.IsExiting)
                {
                    StartMainMenuController();
                    return;
                }

                GameLobbyController.ExitCode exitCode = (GameLobbyController.ExitCode)controller.exitCode;
                switch (exitCode)
                {
                    case GameLobbyController.ExitCode.StartGame:
                    {
                        GameLobbyController glc = controller as GameLobbyController;
                        Scheme selectedScheme = glc.GetSelectedScheme();

                        PlayerInput[] inputs = glc.CreateInputEntries();
                        for (int i = 0; i < inputs.Length; ++i)
                        {
                            Player player = new Player(i);
                            player.SetPlayerInput(inputs[i]);
                            game.AddPlayer(player);
                        }

                        GameSettings settings = new GameSettings(selectedScheme);
                        StartController(GameController.Local(game, settings));
                        break;
                    }
                }
            }
            else if (controller is DebugMultiplayerController)
            {
                if (controller.IsExiting)
                {
                    StartMainMenuController();
                    return;
                }

                DebugMultiplayerController.ExitCode exitCode = (DebugMultiplayerController.ExitCode)controller.exitCode;
                switch (exitCode)
                {
                    case DebugMultiplayerController.ExitCode.ClientStarted:
                    {
                        ServerInfo info = controller.exitData as ServerInfo;
                        Assert.IsTrue(info != null);

                        game.AddPlayer(new Player(), InputMapping.CreatePlayerInput(InputType.Keyboard1));

                        GameSettings settings = new GameSettings(info.scheme);
                        StartController(GameController.Client(game, settings));
                        break;
                    }

                    case DebugMultiplayerController.ExitCode.ServerStarted:
                    {
                        ServerInfo info = controller.exitData as ServerInfo;
                        Assert.IsTrue(info != null);

                        game.AddPlayer(new Player(0), InputMapping.CreatePlayerInput(InputType.Keyboard1));

                        GameSettings settings = new GameSettings(info.scheme);
                        StartController(GameController.Server(game, settings));
                        break;
                    }
                }
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Console

        protected override CConsole CreateConsole()
        {
            CConsole console = base.CreateConsole();
            CVars.Register(console);
            return console;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Key listener

        public override bool OnKeyPressed(KeyEventArg e)
        {
            if (base.OnKeyPressed(e))
            {
                return true;
            }

            if (e.key == KeyCode.Oem8)
            {
                ToggleConsole();
                return true;
            }

            return false;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        private void StartMainMenuController()
        {
            m_menuController = new MenuController();
            StartController(m_menuController);
        }

        public NetworkManager GetNetwork()
        {
            return m_networkManager;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public static BmRootController Current
        {
            get { return BmApplication.RootController(); }
        }

        public Game game
        {
            get { return m_game; }
        }

        #if UNIT_TESTING

        protected NetworkManager networkManager
        {
            get { return m_networkManager; }
            set { m_networkManager = value; }
        }

        #endif

        #endregion
    }
}
