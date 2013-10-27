using System.IO;

namespace BomberEngine
{
    public interface AssetReader
    {
        Asset Read(Stream stream);
    }
}
