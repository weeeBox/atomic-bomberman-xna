using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Assets
{
    public interface AssetManagerListener
    {
        void OnResourceLoaded(AssetManager manager, AssetLoadInfo info);

        void OnResourcesLoaded(AssetManager manager);
    }
}
