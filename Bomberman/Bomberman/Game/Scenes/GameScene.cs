using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using Assets;
using BomberEngine.Core.Assets.Types;
using Bomberman.Game.Elements;
using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Scenes
{
    public class GameScene : Scene
    {
        private static readonly int FIELD_OFFSET_X = 20;
        private static readonly int FIELD_OFFSET_Y = 68;
        private static readonly int FIELD_WIDTH = 600;
        private static readonly int FIELD_HEIGHT = 396;

        private Image fieldBackground;

        private Field field;

        public GameScene()
        {
            field = new Field(15, 11);

            // field background
            fieldBackground = Helper.CreateImage(A.tex_FIELD7);
            AddDrawable(fieldBackground);

            // field drawer
            AddDrawable(new FieldDrawable(field, FIELD_OFFSET_X, FIELD_OFFSET_Y, FIELD_WIDTH, FIELD_HEIGHT));
        }
    }
}
