using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BomberEngine.Core.Assets.Types;
using Microsoft.Xna.Framework.Graphics;

namespace BomberEngine.Core.Assets.Readers
{
    public class TextureReader : AssetReader
    {
        public Asset Read(Stream stream)
        {   
            Texture2D texture = Texture2D.FromStream(Global.graphicsDevice, stream);
            return new TextureImage(texture);
        }
    }
}
