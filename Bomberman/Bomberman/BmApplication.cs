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
    public class BmApplication : Application
    {
        private ContentManager contentManager;

        public BmApplication(ContentManager contentManager, ApplicationInfo info)
            : base(info)
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
            return new BmAssetManager(contentManager);
        }

        protected override RootController CreateRootController()
        {
            return new BmRootController(contentManager);
        }

        public static new BmAssetManager Assets()
        {
            return Application.Assets() as BmAssetManager;
        }

        public static new BmRootController RootController()
        {
            return Application.RootController() as BmRootController;
        }
    }
}
