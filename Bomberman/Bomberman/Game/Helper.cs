using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets;
using BomberEngine.Core.Visual;
using BomberEngine.Game;
using BomberEngine.Core.Assets.Types;

namespace Bomberman.Game
{
    public class Helper
    {
        public static TextureImage GetTexture(int id)
        {
            return Application.Assets().GetTexture(id);
        }

        public static Image CreateImage(int id)
        {
            TextureImage texture = GetTexture(id);
            return new Image(texture);
        }
    }
}
