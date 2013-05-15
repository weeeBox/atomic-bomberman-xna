using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BomberEngine.Core.Visual;

namespace BomberEngine.Core.Assets.Loaders
{
    public class VectorFontLoader : AssetLoader
    {
        public Asset LoadAsset(ContentManager contentManager, AssetLoadInfo info)
        {
            SpriteFont font = contentManager.Load<SpriteFont>(info.GetAssetContentName());
            return new VectorFont(font);
        }
    }
}
