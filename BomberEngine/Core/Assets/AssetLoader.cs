using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace BomberEngine.Core.Assets
{
    public interface AssetLoader
    {
        Asset LoadAsset(ContentManager contentManager, AssetLoadInfo info);
    }
}
