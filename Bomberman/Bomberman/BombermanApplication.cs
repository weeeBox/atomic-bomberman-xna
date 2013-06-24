using BomberEngine.Core.Assets;
using BomberEngine.Debugging;
using BomberEngine.Game;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Native;

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

        protected override AssetManager CreateAssetManager()
        {
            return new BombermanAssetManager(contentManager);
        }

        protected override RootController CreateRootController()
        {
            return new BombermanRootController(contentManager);
        }
    }
}
