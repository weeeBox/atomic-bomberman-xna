using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Assets;
using Microsoft.Xna.Framework.Content;

namespace Bomberman.Content.Loader
{
    public class AnimationLoader : AssetLoader
    {
        public Asset LoadAsset(ContentManager contentManager, AssetLoadInfo info)
        {
            return contentManager.Load<Animation>(info.GetAssetContentName());
        }
    }
}
