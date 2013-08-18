using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Assets.Types;
using Bomberman.UI;
using Bomberman.Game.Elements.Players;

namespace Bomberman.Game.Screens
{
    public class RoundResultScreen : Screen
    {
        public enum ButtonId
        {
            Exit,
            Continue
        }

        private Game m_game;

        public RoundResultScreen(Game game, ButtonDelegate buttonDelegate)
        {
            m_game = game;

            View contentView = new View(64, 48, 512, 384);

            // table
            View tableView = new View(0, 25, contentView.width, 330);

            float nameColWidth = 320;
            float winsColWidth = 0.5f * (tableView.width - nameColWidth);
            float suicidesColWidth = winsColWidth;

            Font font = Helper.fontButton;

            TextView textView = new TextView(font, "PLAYER");
            textView.alignX = View.ALIGN_CENTER;
            textView.x = 0.5f * nameColWidth;
            tableView.AddView(textView);

            textView = new TextView(font, "WINS");
            textView.alignX = View.ALIGN_CENTER;
            textView.x = nameColWidth + 0.5f * winsColWidth;
            tableView.AddView(textView);

            textView = new TextView(font, "SUICIDES");
            textView.alignX = View.ALIGN_CENTER;
            textView.x = nameColWidth + winsColWidth + 0.5f * suicidesColWidth;
            tableView.AddView(textView);

            List<Player> players = game.GetPlayersList();
            float px = 0.5f * nameColWidth;
            float py = textView.y + textView.height + 10;
            for (int i = 0; i < players.Count; ++i)
            {
                PlayerResultView pv = new PlayerResultView(players[i]);
                pv.alignX = View.ALIGN_CENTER;
                pv.x = px;
                pv.y = py;

                tableView.AddView(pv);

                py += pv.height + 10;
            }

            contentView.AddView(tableView);

            // buttons
            View buttons = new View(0.5f * contentView.width, contentView.height, 0, 0);
            buttons.alignX = View.ALIGN_CENTER;
            buttons.alignY = View.ALIGN_MAX;

            Button button = new TempButton("EXIT");
            button.id = (int)ButtonId.Exit;
            button.buttonDelegate = buttonDelegate;
            SetCancelButton(button);
            buttons.AddView(button);

            button = new TempButton("START!");
            button.id = (int)ButtonId.Continue; ;
            button.buttonDelegate = buttonDelegate;
            FocusView(button);

            SetConfirmButton(button);
            buttons.AddView(button);

            buttons.LayoutHor(20);
            buttons.ResizeToFitViews();
            contentView.AddView(buttons);

            AddView(contentView);
        }
    }

    class PlayerResultView : View
    {
        private Player m_player;
        private TextView m_readyTextView;

        public PlayerResultView(Player player)
        {
            m_player = player;

            Font font = Helper.fontButton;

            m_readyTextView = new TextView(font, "Not Ready");
            UpdateState();
            AddView(m_readyTextView);

            ResizeToFitViews();
        }

        public override void Update(float delta)
        {
            UpdateState();
        }

        private void UpdateState()
        {
            m_readyTextView.SetText(m_player.IsReady ? "Ready" : "Not Ready");
        }
    }
}
