using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Assets;
using Microsoft.Xna.Framework.Content;
using Assets;

namespace Bomberman
{
    public class BombermanAssetManager : AssetManager
    {
        public BombermanAssetManager(ContentManager contentManager)
            : base(contentManager, A.RES_COUNT)
        {
        }

        public void AddPackToLoad(AssetPacks.Packs pack)
        {
            AssetLoadInfo[] infos = AssetPacks.GetPack(pack);
            foreach (AssetLoadInfo info in infos)
            {
                AddToQueue(info);
            }
        }
    }
}
