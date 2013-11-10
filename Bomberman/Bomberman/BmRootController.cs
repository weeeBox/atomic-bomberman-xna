using System;
using Assets;
using BomberEngine;
using Bomberman.Content;
using Bomberman.Gameplay;
using Bomberman.Menu;
using Bomberman.Multiplayer;
using Bomberman.Networking;
using Microsoft.Xna.Framework.Content;

namespace Bomberman
{
    public class BmRootController : RootController
    {
        private MenuController menuController;
        private NetworkManager networkManager;

        private Game m_game;
        private InputMapping m_mapping;

        public BmRootController(ContentManager contentManager)
            : base(contentManager)
        {
            networkManager = new NetworkManager();
            m_mapping = new InputMapping();
            m_game = new Game();
        }

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
                    menuController.Stop((int)MenuController.ExitCode.DebugClientStart);
                }
                else if (mode.Equals("server"))
                {
                    menuController.Stop((int)MenuController.ExitCode.DebugServerStart);
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

        #region IUpdatable

        public override void Update(float delta)
        {
            base.Update(delta);
            networkManager.Update(delta);
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

                        GameSettings settings = new GameSettings(selectedScheme);
                        settings.inputEntries = glc.CreateInputEntries();
                        StartController(GameController.Local(settings));
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

                        GameSettings settings = new GameSettings(info.scheme);
                        settings.inputEntries = new GameSettings.InputEntry[]
                        {
                            new GameSettings.InputEntry(0, InputMapping.CreatePlayerInput(InputType.Keyboard1))
                        };

                        StartController(GameController.Client(settings));
                        break;
                    }

                    case DebugMultiplayerController.ExitCode.ServerStarted:
                    {
                        ServerInfo info = controller.exitData as ServerInfo;
                        Assert.IsTrue(info != null);

                        GameSettings settings = new GameSettings(info.scheme);
                        settings.inputEntries = new GameSettings.InputEntry[]
                        {
                            new GameSettings.InputEntry(0, InputMapping.CreatePlayerInput(InputType.Keyboard1))
                        };
                        StartController(GameController.Server(settings));
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
            menuController = new MenuController();
            StartController(menuController);
        }

        public NetworkManager GetNetwork()
        {
            return networkManager;
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

        #endregion
    }
}
