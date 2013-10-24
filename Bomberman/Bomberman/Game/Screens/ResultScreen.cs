using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Assets.Types;
using Bomberman.UI;
using Bomberman.Game.Elements.Players;
using Microsoft.Xna.Framework;

namespace Bomberman.Game.Screens
{
    public class BaseResultScreen : Screen
    {
        public enum ButtonId
        {
            Exit,
            Continue
        }

        protected View contentView;

        protected BaseResultScreen(Game game)
        {
            contentView = new View(64, 48, 512, 384);

            // table
            View tableView = new View(0, 25, contentView.width, 330);
            tableView.debugColor = Color.Red;

            float indent = 20;
            float suicidesColWidth = 94;
            float winsColWidth = 54;
            float killsColWidth = 54;
            float nameColWidth = tableView.width - (indent + winsColWidth + indent + killsColWidth + indent + suicidesColWidth);

            float nameColX = 0.5f * nameColWidth;
            float winsColX = nameColX + 0.5f * (nameColWidth + winsColWidth) + indent;
            float killsColX = winsColX + 0.5f * (winsColWidth + killsColWidth) + indent;
            float suicidesColX = killsColX + 0.5f * (killsColWidth + suicidesColWidth) + indent;

            Font font = Helper.fontButton;

            // header
            TextView textView = new TextView(font, "PLAYER");
            textView.alignX = View.ALIGN_CENTER;
            textView.x = nameColX;
            tableView.AddView(textView);

            textView = new TextView(font, "WINS");
            textView.alignX = View.ALIGN_CENTER;
            textView.x = winsColX;
            tableView.AddView(textView);

            textView = new TextView(font, "KILLS");
            textView.alignX = View.ALIGN_CENTER;
            textView.x = killsColX;
            tableView.AddView(textView);

            textView = new TextView(font, "SUICIDES");
            textView.alignX = View.ALIGN_CENTER;
            textView.x = suicidesColX;
            tableView.AddView(textView);

            // data
            List<Player> players = game.GetPlayersList();
            float px = 0;
            float py = textView.y + textView.height + indent;
            for (int i = 0; i < players.Count; ++i)
            {
                PlayerResultView pv = new PlayerResultView(players[i], tableView.width);
                pv.debugColor = Color.Green;

                pv.readyTextView.x = nameColX;
                pv.winsView.x = winsColX;
                pv.killsView.x = killsColX;
                pv.suicidesView.x = suicidesColX;

                pv.x = px;
                pv.y = py;

                tableView.AddView(pv);

                py += pv.height + 10;
            }

            contentView.AddView(tableView);

            AddView(contentView);
        }
    }

    class PlayerResultView : View
    {
        private Player m_player;

        public TextView readyTextView;
        public TextView winsView;
        public TextView killsView;
        public TextView suicidesView;

        public PlayerResultView(Player player, float width)
            : base(width, 0)
        {
            m_player = player;

            Font font = Helper.fontButton;

            readyTextView = new TextView(font);
            readyTextView.alignX = View.ALIGN_CENTER;
            AddView(readyTextView);

            winsView = new TextView(font);
            winsView.alignX = View.ALIGN_CENTER;
            AddView(winsView);

            killsView = new TextView(font);
            killsView.alignX = View.ALIGN_CENTER;
            AddView(killsView);

            suicidesView = new TextView(font);
            suicidesView.alignX = View.ALIGN_CENTER;
            AddView(suicidesView);

            ResizeToFitViewsVer();
        }

        public override void Update(float delta)
        {
            UpdateState(); // TODO: send notification
        }

        private void UpdateState()
        {
            readyTextView.SetText(m_player.IsReady ? "Ready" : "Not Ready");

            PlayerStatistics statistics = m_player.statistics;
            winsView.SetText(statistics.winsCount);
            killsView.SetText(statistics.killsCount);
            suicidesView.SetText(statistics.suicidesCount);
        }
    }
}
