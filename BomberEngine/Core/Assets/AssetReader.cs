using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BomberEngine.Core.Assets
{
    public interface AssetReader
    {
        Asset Read(Stream stream);
    }
}
