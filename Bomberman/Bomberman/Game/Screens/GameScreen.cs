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
    }
}
