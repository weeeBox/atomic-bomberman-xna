using BomberEngine.Core.Assets;
using BomberEngine.Debugging;
using BomberEngine.Game;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;

namespace Bomberman
{
    public class BombermanApplication : Application, KeyboardListener
    {
        private ContentManager contentManager;

        public BombermanApplication(ContentManager contentManager, int width, int height) : base(width, height)
        {
            this.contentManager = contentManager;
        }

        protected override AssetManager CreateAssetManager()
        {
            return new BombermanAssetManager(contentManager);
        }

        protected override RootController CreateRootController()
        {
            return new BombermanRootController();
        }

        protected override GameConsole CreateConsole()
        {
            SpriteFont consoleFont = contentManager.Load<SpriteFont>("ConsoleFont");
            return new GameConsole(consoleFont);
        }

        //////////////////////////////////////////////////////////////////////////////

        protected override void OnStart()
        {
            Input().AddKeyboardListener(this);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region KeyboardListener

        public void KeyPressed(Keys key)
        {
            if (key == Keys.Oem8)
            {
                ToggleConsole();
            }
        }

        public void KeyReleased(Keys key)
        {   
        }

        #endregion
    }
}
