using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Assets;

namespace Assets
{
    public class AssetType : AssetTypeBase
    {
        public const int Scheme = AssetTypeBase.BaseCount + 1;
        public const int Animation = AssetTypeBase.BaseCount + 2;
    }
}
