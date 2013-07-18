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
using BomberEngine.Core.Input;

namespace Bomberman.Game.Screens
{
    public class GameScreen : Screen
    {
        private ImageView fieldBackground;

        public GameScreen()
        {
            Field field = Game.Field();
            AddUpdatable(field);

            // field background
            fieldBackground = Helper.CreateImage(A.gfx_field7);
            AddView(fieldBackground);

            // field drawer
            AddView(new FieldDrawable(field, Constant.FIELD_OFFSET_X, Constant.FIELD_OFFSET_Y, Constant.FIELD_WIDTH, Constant.FIELD_HEIGHT));

            // field drawer
            AddView(new PowerupsDrawable(field, 0, 0, Constant.FIELD_WIDTH, Constant.FIELD_OFFSET_Y));

            List<Player> players = field.GetPlayers().list;
            foreach (Player player in players)
            {
                if (player.input is PlayerKeyboardInput)
                {
                    PlayerKeyboardInput kb = (PlayerKeyboardInput)player.input;
                    AddKeyListener(kb);
                }
                else if (player.input is PlayerGamePadInput)
                {
                    PlayerGamePadInput gp = (PlayerGamePadInput)player.input;
                    AddUpdatable(gp);
                }
            }
        }

        protected override bool OnBackKeyPressed(KeyEventArg arg)
        {
            GameController gc = CurrentController as GameController;
            gc.ShowPauseScreen();
            return true;
        }
    }
}
