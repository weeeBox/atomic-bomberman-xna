using System;
using Assets;
using BomberEngine.Consoles;
using BomberEngine.Core.Input;
using BomberEngine.Game;
using Bomberman.Game;
using Bomberman.Menu;
using Bomberman.Multiplayer;
using Bomberman.Networking;
using Microsoft.Xna.Framework.Content;
using System.Net;
using BomberEngine.Debugging;
using Bomberman.Content;
using Bomberman.Game.Elements.Players.Input;

namespace Bomberman
{
    public class BombermanRootController : RootController
    {
        private MenuController menuController;
        private MultiplayerManager multiplayerManager;

        public BombermanRootController(ContentManager contentManager)
            : base(contentManager)
        {
            multiplayerManager = new MultiplayerManager();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        protected override void OnStart()
        {
            BombermanAssetManager manager = (BombermanAssetManager)Application.Assets();
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

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region IUpdatable

        public override void Update(float delta)
        {
            base.Update(delta);
            multiplayerManager.Update(delta);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Child controllers

        protected override void OnControllerStop(Controller controller)
        {
            if (controller is MenuController)
            {
                MenuController.ExitCode exitCode = (MenuController.ExitCode)controller.exitCode;
                if (exitCode == MenuController.ExitCode.Quit)
                {
                    Application.sharedApplication.Stop();
                    return;
                }

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
                }
            }
            else if (controller is MultiplayerController)
            {
                MultiplayerController.ExitCode exitCode = (MultiplayerController.ExitCode)controller.exitCode;
                if (exitCode == MultiplayerController.ExitCode.Cancel)
                {
                    StartMainMenuController();
                    return;
                }

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

                    case GameLobbyController.ExitCode.Cancel:
                    {
                        StartMainMenuController();
                        break;
                    }
                }
            }
            else if (controller is DebugMultiplayerController)
            {
                DebugMultiplayerController.ExitCode exitCode = (DebugMultiplayerController.ExitCode)controller.exitCode;
                switch (exitCode)
                {
                    case DebugMultiplayerController.ExitCode.Cancel:
                    {
                        StartMainMenuController();
                        break;
                    }
                    case DebugMultiplayerController.ExitCode.ClientStarted:
                    {
                        ServerInfo info = controller.exitData as ServerInfo;
                        Debug.Assert(info != null);

                        GameSettings settings = new GameSettings(info.scheme);
                        StartController(GameController.Client(settings));
                        break;
                    }

                    case DebugMultiplayerController.ExitCode.ServerStarted:
                    {
                        ServerInfo info = controller.exitData as ServerInfo;
                        Debug.Assert(info != null);

                        GameSettings settings = new GameSettings(info.scheme);
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

        public MultiplayerManager GetMultiplayerManager()
        {
            return multiplayerManager;
        }

        #endregion
    }
}
