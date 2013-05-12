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
using Microsoft.Xna.Framework.Input;

namespace Bomberman.Game.Scenes
{
    public class GameScene : Scene
    {
        private Image fieldBackground;

        public GameScene()
        {
            Field field = Game.Field();
            AddUpdatabled(field);

            // field background
            fieldBackground = Helper.CreateImage(A.tex_FIELD7);
            AddChild(fieldBackground);

            // field drawer
            AddChild(new FieldDrawable(field, Constant.FIELD_OFFSET_X, Constant.FIELD_OFFSET_Y, Constant.FIELD_WIDTH, Constant.FIELD_HEIGHT));

            // field drawer
            AddChild(new PowerupsDrawable(field, 0, 0, Constant.FIELD_WIDTH, Constant.FIELD_OFFSET_Y));
        }

        public override void KeyPressed(Keys key)
        {
            base.KeyPressed(key);

            if (key == Keys.Escape)
            {
                PauseScene pauseScene = new PauseScene();
                StartNextScene(pauseScene);
            }
        }
    }
}
