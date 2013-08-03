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
using BomberEngine.Core.Events;
using BomberEngine.Debugging;

namespace Bomberman.Game.Screens
{
    public class GameScreen : Screen
    {
        private ImageView fieldBackground;        

        public GameScreen()
        {
            Field field = Game.Current.Field;
            
            AddUpdatable(field);

            // field background
            fieldBackground = Helper.CreateImage(A.gfx_field7);
            AddView(fieldBackground);

            // field drawer
            AddView(new FieldDrawable(field, Constant.FIELD_OFFSET_X, Constant.FIELD_OFFSET_Y, Constant.FIELD_WIDTH, Constant.FIELD_HEIGHT));

            // field drawer
            AddView(new PowerupsView(field, 0, 0, Constant.FIELD_WIDTH, Constant.FIELD_OFFSET_Y));

            // HACK: disable updating keyboard input when console is showing
            SetKeyboardInputActive(!Application.RootController().Console.IsVisible);
        }

        protected override void OnStart()
        {
            RegisterNotification(Notifications.ConsoleVisiblityChanged, ConsoleVisiblityChangedNotification);
        }

        protected override bool OnCancelPressed(KeyEventArg arg)
        {
            GameController gc = CurrentController as GameController;
            gc.ShowPauseScreen();
            return true;
        }

        private void ConsoleVisiblityChangedNotification(Notification notification)
        {
            bool visible = notification.boolData;
            SetKeyboardInputActive(!visible);
        }

        private static void SetKeyboardInputActive(bool active)
        {
            Field field = Field.Current();
            List<Player> players = field.GetPlayers().list;
            foreach (Player player in players)
            {
                PlayerInput input = player.input;
                if (input is PlayerKeyInput)
                {
                    input.IsActive = active;
                }
            }
        }
    }
}
