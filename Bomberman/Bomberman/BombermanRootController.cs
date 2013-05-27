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
using BomberEngine.Consoles;

namespace Bomberman
{
    public class BombermanRootController : RootController
    {
        private MenuController menuController;
        private GameController gameController;

        public BombermanRootController(ContentManager contentManager)
            : base(contentManager)
        {   
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

        protected override Cmd CreateConsole()
        {
            Cmd console = base.CreateConsole();
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
