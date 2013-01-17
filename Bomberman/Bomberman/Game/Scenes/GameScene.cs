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
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements.Players;
using Bomberman.Game.Elements.Players.Input;
using Bomberman.Content;

namespace Bomberman.Game.Scenes
{
    public class GameScene : Scene
    {
        private Image fieldBackground;

        private Field field;

        public GameScene()
        {
            field = LoadField();

            KeyboardPlayerInput input = new KeyboardPlayerInput();
            Application.Input().SetKeyboardListener(input);

            Player player = new Player(input, 0, 0);
            field.AddPlayer(player);

            AddUpdatabled(field);

            // field background
            fieldBackground = Helper.CreateImage(A.tex_FIELD7);
            AddDrawable(fieldBackground);

            // field drawer
            AddDrawable(new FieldDrawable(field, Constant.FIELD_OFFSET_X, Constant.FIELD_OFFSET_Y, Constant.FIELD_WIDTH, Constant.FIELD_HEIGHT));
        }

        private Field LoadField()
        {
            Scheme scheme = Helper.GetScheme(A.sch_X);
            return new Field(scheme);
        }
    }
}
