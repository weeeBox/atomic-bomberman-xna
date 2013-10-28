using System.Collections.Generic;
using Assets;
using BomberEngine;
using Bomberman.Game.Elements.Fields;
using Bomberman.Game.Elements.Players;

namespace Bomberman.Game.Screens
{
    public class GameScreen : Screen
    {
        private ImageView fieldBackground;
        private Field m_field;

        public GameScreen()
        {
            Field field = Game.Current.Field;
            m_field = field;
            
            AddUpdatable(field);

            // field background
            fieldBackground = Helper.CreateImage(A.gfx_field7);
            AddView(fieldBackground);

            // field drawer
            AddView(new FieldDrawable(field, Constant.FIELD_OFFSET_X, Constant.FIELD_OFFSET_Y, Constant.FIELD_WIDTH, Constant.FIELD_HEIGHT));

            // powerups info
            AddPowerupsView();

            // HACK: disable updating keyboard input when console is showing
            SetKeyboardInputActive(!Application.RootController().Console.IsVisible);
        }

        protected override void OnStart()
        {
            RegisterNotification(Notifications.ConsoleVisiblityChanged, ConsoleVisiblityChangedNotification);
        }

        protected override void OnStop()
        {
            m_field.CancelAllTimers();
            base.OnStop();
        }

        protected override bool OnCancelPressed(KeyEventArg arg)
        {
            GameController gc = CurrentController as GameController;
            gc.ShowPauseScreen();
            return true;
        }

        [System.Diagnostics.Conditional("DEBUG_VIEW")]
        private void AddPowerupsView()
        {
            View container = new View();
            List<Player> players = Field.PlayersList;
            for (int i = 0; i < players.Count; ++i)
            {
                container.AddView(new PowerupsView(players[i].powerups));
            }
            container.LayoutVer(0);
            container.ResizeToFitViews();
            AddDebugView(container);
        }

        private void ConsoleVisiblityChangedNotification(Notification notification)
        {
            bool visible = notification.GetData<bool>();
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
