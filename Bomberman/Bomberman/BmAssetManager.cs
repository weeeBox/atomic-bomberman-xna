using Assets;
using BomberEngine.Core.Assets;
using Bomberman.Content;
using Microsoft.Xna.Framework.Content;

namespace Bomberman
{
    public class BmAssetManager : AssetManager
    {
        public BmAssetManager(ContentManager contentManager)
            : base("Assets", A.RES_COUNT)
        {
            RegisterReader(typeof(Scheme), new SchemeReader());
            RegisterReader(typeof(Animation), new AnimationReader());
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

        public Animation GetAnimation(int id)
        {
            return (Animation)GetAsset(id);
        }
    }
}
