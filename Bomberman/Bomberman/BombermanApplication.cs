using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using Microsoft.Xna.Framework;
using Bomberman.Game;
using Microsoft.Xna.Framework.Content;
using BomberEngine.Core.Assets;
using Assets;

namespace Bomberman
{
    public class BombermanApplication : Application
    {
        private ContentManager contentManager;

        public BombermanApplication(ContentManager contentManager, GraphicsDeviceManager graphics) : base(graphics)
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
    }
}
