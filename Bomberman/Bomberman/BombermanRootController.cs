using Assets;
using BomberEngine.Debugging;
using BomberEngine.Game;
using Bomberman.Game;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Bomberman.Game.Commands;
using Bomberman.Menu;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Assets.Types;
using BomberEngine.Core.Input;

namespace Bomberman
{
    public class BombermanRootController : RootController
    {
        private MenuController menuController;
        private GameController gameController;
        private ContentManager contentManager;

        public BombermanRootController(ContentManager contentManager)
        {
            this.contentManager = contentManager;
        }

        protected override void OnStart()
        {
            BombermanAssetManager manager = (BombermanAssetManager)Application.Assets();
            manager.AddPackToLoad(AssetPacks.Packs.ALL);
            manager.LoadImmediately();

            menuController = new MenuController();
            StartController(menuController);
        }

        protected override void OnControllerStop(Controller controller)
        {
            switch (controller.exitCode)
            {
                case ExitCode.StartGame:
                    gameController = new GameController();
                    StartController(gameController);
                    break;

                case ExitCode.Quit:
                    Application.sharedApplication.RunStop();
                    break;
            }
        }

        protected override GameConsole CreateConsole()
        {
            Font consoleFont = new VectorFont(contentManager.Load<SpriteFont>("ConsoleFont"));
            GameConsole console = new GameConsole(consoleFont);

            console.RegisterCommand(new PowerupAddCommand());
            console.RegisterCommand(new PowerupRemoveCommand());
            console.RegisterCommand(new PowerupListCommand());
            console.RegisterCommand(new PlayerAddCommand());
            console.RegisterCommand(new PlayerRemoveCommand());
            console.RegisterCommand(new DiseaseInfectCommand());
            console.RegisterCommand(new DiseaseListCommand());

            return console;
        }

        public override bool OnKeyPressed(KeyEventArg e)
        {
            if (base.OnKeyPressed(e))
            {
                return true;
            }

            if (e.key == KeyCode.KB_Oem8)
            {
                ToggleConsole();
                return true;
            }

            return false;
        }
    }
}
