using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Assets
{
    public struct AssetLoadInfo
    {
        public readonly int id;
        public readonly String path;
        public readonly Type type;

        public AssetLoadInfo(int id, Type type, String path)
        {
            this.id = id;
            this.type = type;
            this.path = path;
        }
    }
}
