using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Assets.Types;
using Assets;

namespace Bomberman.Game.Screens
{
    public class LobbyScreen : Screen
    {
        public enum ButtonId
        {
            Back,
            Start,
        }

        public LobbyScreen(ButtonDelegate buttonDelegate)
        {
            View contentView = new View(512, 363);
            contentView.alignX = View.ALIGN_CENTER;
            contentView.x = 0.5f * width;
            contentView.y = 48;

            Font font = Helper.GetFont(A.fnt_button);

            contentView.AddView(new View(215, 145));

            TextView serverName = new TextView(font, "SERVER NAME");
            serverName.alignX = View.ALIGN_CENTER;
            serverName.x = 113;
            serverName.y = 155;

            contentView.AddView(serverName);

            View listView = new View(286, contentView.height);
            listView.alignX = View.ALIGN_MAX;
            listView.x = contentView.width;
            contentView.AddView(listView);

            AddView(contentView);

            View buttons = new View();
            buttons.x = 0.5f * width;
            buttons.y = 432;
            buttons.alignX = View.ALIGN_CENTER;
            buttons.alignY = View.ALIGN_MAX;

            Button button = new TextButton("BACK", 0, 0, 100, 20);
            button.SetDelegate(buttonDelegate);
            button.id = (int)ButtonId.Back;
            buttons.AddView(button);

            button = new TextButton("START!", 0, 0, 100, 20);
            button.SetDelegate(buttonDelegate);
            button.id = (int)ButtonId.Start;
            buttons.AddView(button);

            buttons.LayoutHor(10);
            buttons.ResizeToFitViews();
            AddView(buttons);
        }
    }
}
