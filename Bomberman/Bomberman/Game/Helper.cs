using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets;
using BomberEngine.Core.Visual;
using BomberEngine.Game;
using BomberEngine.Core.Assets.Types;
using Bomberman.Content;

namespace Bomberman.Game
{
    public class Helper
    {
        public static TextureImage GetTexture(int id)
        {
            return Application.Assets().GetTexture(id);
        }

        public static Font GetFont(int id)
        {
            return Application.Assets().GetFont(id);
        }

        public static Scheme GetScheme(int id)
        {
            return ((BombermanAssetManager)Application.Assets()).GetScheme(id);
        }

        public static ImageView CreateImage(int id)
        {
            TextureImage texture = GetTexture(id);
            return new ImageView(texture);
        }
    }
}
