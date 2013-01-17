using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using BombermanCommon.Resources.Scheme;
using Bomberman.Content;

namespace BomberEngine.Core.Assets.Loaders
{
    public class SchemeLoader : AssetLoader
    {
        public Asset LoadAsset(ContentManager contentManager, AssetLoadInfo info)
        {
            SchemeInfo schemeInfo = contentManager.Load<SchemeInfo>(info.GetAssetContentName());
            return new Scheme(schemeInfo);
        }
    }
}
