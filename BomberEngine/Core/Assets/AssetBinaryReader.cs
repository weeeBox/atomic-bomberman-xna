using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BomberEngine.Core.Assets
{
    public abstract class AssetBinaryReader : AssetReader
    {
        public Asset Read(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                return Read(reader);
            }
        }

        protected abstract Asset Read(BinaryReader input);
    }
}
