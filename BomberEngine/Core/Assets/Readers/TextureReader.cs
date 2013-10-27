using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace BomberEngine
{
    public class TextureReader : AssetReader
    {
        public Asset Read(Stream stream)
        {   
            Texture2D texture = Texture2D.FromStream(Runtime.graphicsDevice, stream);
            return new TextureImage(texture);
        }
    }
}
