using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Assets
{
    public struct AssetLoadInfo
    {
        public String path;
        public int index;
        public AssetType type;

        public AssetLoadInfo(String path, int index, AssetType type)
        {
            this.path = path;
            this.index = index;
            this.type = type;
        }
    }
}
