using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using Assets;
using BomberEngine.Core.Assets.Types;

namespace Bomberman.Game.Scenes
{
    public class GameScene : Scene
    {
        public GameScene()
        {
            TextureImage texture = Application.Assets().GetTexture(A.tex_WLKS0001);
            Image image = new Image(texture);
            image.x = 100;
            image.y = 200;

            AddDrawable(image);
        }
    }
}
