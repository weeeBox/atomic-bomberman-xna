using BomberEngine.Core.Assets;
using BomberEngine.Debugging;
using BomberEngine.Game;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;
using Bomberman.Game.Commands.Gameplay.Powerups;
using Bomberman.Game.Commands.Gameplay.Players;

namespace Bomberman
{
    public class BombermanApplication : Application
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
            return new BombermanRootController(contentManager);
        }
    }
}
