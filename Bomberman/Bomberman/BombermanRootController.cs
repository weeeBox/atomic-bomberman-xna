using Assets;
using BomberEngine.Debugging;
using BomberEngine.Game;
using Bomberman.Game;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Bomberman.Game.Commands;
using Bomberman.Menu;

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
            BombermanAssetManager manager = (BombermanAssetManager) Application.Assets();
            manager.AddPackToLoad(AssetPacks.Packs.ALL);
            manager.LoadImmediately();

            menuController = new MenuController();
            StartController(menuController);
        }

        protected override GameConsole CreateConsole()
        {
            SpriteFont consoleFont = contentManager.Load<SpriteFont>("ConsoleFont");
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

        public override bool OnKeyPressed(Keys key)
        {
            if (base.OnKeyPressed(key))
            {
                return true;
            }

            if (key == Keys.Oem8)
            {
                ToggleConsole();
                return true;
            }

            return false;
        }
    }
}
