using System.IO;

namespace BomberEngine
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
