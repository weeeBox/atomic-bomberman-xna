using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BomberEngine.Core.Assets.Types;

namespace BomberEngine.Core.Assets.Loaders
{
    public class TextureLoader : AssetLoader
    {
        public Asset LoadAsset(ContentManager contentManager, AssetLoadInfo info)
        {
            Texture2D texture = contentManager.Load<Texture2D>(info.GetAssetContentName());
            return new TextureImage(texture);
        }
    }
}
