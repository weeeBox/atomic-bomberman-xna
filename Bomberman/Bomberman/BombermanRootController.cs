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

        protected override void OnStart()
        {
            BombermanAssetManager manager = (BombermanAssetManager)Application.Assets();
            manager.AddPackToLoad(AssetPacks.Packs.ALL);
            manager.LoadImmediately();

            StartMainMenu();
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            multiplayerManager.Update(delta);
        }

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
                        throw new NotImplementedException();
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
                    StartMainMenu();
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
                StartMainMenu();
            }
            else if (controller is DebugMultiplayerController)
            {
                DebugMultiplayerController.ExitCode exitCode = (DebugMultiplayerController.ExitCode)controller.exitCode;
                switch (exitCode)
                {
                    case DebugMultiplayerController.ExitCode.Cancel:
                    {
                        StartMainMenu();
                        break;
                    }
                    case DebugMultiplayerController.ExitCode.ClientStarted:
                    {
                        ServerInfo info = controller.exitData as ServerInfo;
                        Debug.Assert(info != null);

                        GameSettings settings = new GameSettings(info.scheme);
                        settings.multiplayer = GameSettings.Multiplayer.Client;
                        StartController(new GameController(settings));
                        break;
                    }

                    case DebugMultiplayerController.ExitCode.ServerStarted:
                    {
                        ServerInfo info = controller.exitData as ServerInfo;
                        Debug.Assert(info != null);

                        GameSettings settings = new GameSettings(info.scheme);
                        settings.multiplayer = GameSettings.Multiplayer.Server;
                        StartController(new GameController(settings));
                        break;
                    }
                }
            }
        }

        protected override CConsole CreateConsole()
        {
            CConsole console = base.CreateConsole();
            CVars.Register(console);
            return console;
        }

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

        private void StartMainMenu()
        {
            menuController = new MenuController();
            StartController(menuController);
        }

        public MultiplayerManager GetMultiplayerManager()
        {
            return multiplayerManager;
        }
    }
}
