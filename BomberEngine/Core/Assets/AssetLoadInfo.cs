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
        public int type;

        public AssetLoadInfo(String path, int index, int type)
        {
            this.path = path;
            this.index = index;
            this.type = type;
        }

        public String GetAssetContentName()
        {
            int dotIndex = path.LastIndexOf('.');
            return dotIndex != -1 ? path.Substring(0, dotIndex) : path;
        }
    }
}
