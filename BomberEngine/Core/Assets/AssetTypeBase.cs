using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Assets
{
    public class AssetTypeBase
    {   
        public const int Texture = 1;
        public const int Sound = 2;
        public const int Music = 3;
        public const int VectorFont = 4;
        public const int PixelFont = 5;

        protected const int BaseCount = 5;
    }
}
