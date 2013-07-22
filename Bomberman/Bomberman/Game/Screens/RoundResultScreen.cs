using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Assets.Types;

namespace Bomberman.Game.Screens
{
    public class RoundResultScreen : Screen
    {
        public enum ButtonId
        {
            Exit,
            Continue
        }

        public RoundResultScreen(ButtonDelegate buttonDelegate)
        {
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

            contentView.AddView(tableView);

            // buttons
            View buttons = new View(0.5f * contentView.width, contentView.height, 0, 0);
            buttons.alignX = View.ALIGN_CENTER;
            buttons.alignY = View.ALIGN_MAX;

            Button button = new TextButton("EXIT", Helper.fontButton, 0, 0, 100, 20);
            button.id = (int)ButtonId.Exit;
            button.buttonDelegate = buttonDelegate;
            SetCancelButton(button);
            buttons.AddView(button);

            button = new TextButton("START!", Helper.fontButton, 0, 0, 100, 20);
            button.id = (int)ButtonId.Continue; ;
            button.buttonDelegate = buttonDelegate;
            SetConfirmButton(button);
            buttons.AddView(button);

            buttons.LayoutHor(20);
            buttons.ResizeToFitViews();
            contentView.AddView(buttons);

            AddView(contentView);
        }
    }
}
