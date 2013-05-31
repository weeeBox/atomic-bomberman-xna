using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BomberEngine
{
    public class Global
    {
        public static GraphicsDevice graphicsDevice;

        public static void Destroy()
        {
            graphicsDevice = null;
        }
    }
}
