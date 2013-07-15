using BomberEngine.Core.Assets;
using BomberEngine.Debugging;
using BomberEngine.Game;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Native;
using Bomberman.Game;
using BomberEngine.Core.Assets.Types;

namespace Bomberman
{
    public class BombermanApplication : Application
    {
        private ContentManager contentManager;

        public BombermanApplication(ContentManager contentManager, INativeInterface nativeInterface, int width, int height)
            : base(nativeInterface, width, height)
        {
            this.contentManager = contentManager;
        }

        protected override void OnStart()
        {
            SpriteFont systemFont = contentManager.Load<SpriteFont>("SystemFont");
            Helper.fontSystem = new VectorFont(systemFont);
            Helper.fontButton = new VectorFont(contentManager.Load<SpriteFont>("ButtonFont"));

            context.SetSystemFont(systemFont);
        }

        protected override AssetManager CreateAssetManager()
        {
            return new BombermanAssetManager(contentManager);
        }

        protected override RootController CreateRootController()
        {
            return new BombermanRootController(contentManager);
        }

        public static new BombermanAssetManager Assets()
        {
            return Application.Assets() as BombermanAssetManager;
        }

        public static new BombermanRootController RootController()
        {
            return Application.RootController() as BombermanRootController;
        }
    }
}
