using Assets;
using BomberEngine.Debugging;
using BomberEngine.Game;
using Bomberman.Game;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Bomberman.Game.Commands;

namespace Bomberman
{
    public class BombermanRootController : RootController
    {
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

            gameController = new GameController();
            StartController(gameController);
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

        public override void KeyPressed(Keys key)
        {
            if (key == Keys.Oem8)
            {
                ToggleConsole();
                return;
            }

            base.KeyPressed(key);
        }
    }
}
