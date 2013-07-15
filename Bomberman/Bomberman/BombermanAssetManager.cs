using Assets;
using BomberEngine.Core.Assets;
using Bomberman.Content;
using Microsoft.Xna.Framework.Content;

namespace Bomberman
{
    public class BombermanAssetManager : AssetManager
    {
        public BombermanAssetManager(ContentManager contentManager)
            : base(A.RES_COUNT)
        {
            RegisterReader(typeof(Scheme), new SchemeReader());
        }

        public void AddPackToLoad(A.Packs pack)
        {
            AssetLoadInfo[] infos = A.GetPack(pack);
            foreach (AssetLoadInfo info in infos)
            {
                AddToQueue(info);
            }
        }

        public Scheme GetScheme(int id)
        {
            return (Scheme)GetAsset(id);
        }
    }
}
