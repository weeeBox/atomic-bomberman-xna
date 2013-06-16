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
    public class GameLobbyScreen : Screen
    {
        private bool local;
        private View contentView;
        private View busyView;

        public GameLobbyScreen(bool local)
        {
            Font font = Helper.GetFont(A.fnt_button);

            TextView headerText = new TextView(font, "LOCAL SERVERS");
            headerText.alignX = View.ALIGN_CENTER;
            headerText.x = 0.5f * width;
            headerText.y = 31;
            AddView(headerText);

            contentView = new View(467, 333);
            contentView.alignX = View.ALIGN_CENTER;
            contentView.x = 0.5f * width;
            contentView.y = 73;
            contentView.visible = false;
            AddView(contentView);

            busyView = new View(467, 333);
            busyView.alignX = View.ALIGN_CENTER;
            busyView.x = 0.5f * width;
            busyView.y = 73;

            TextView busyText = new TextView(font, "Searching for local servers...");
            busyText.alignX = busyText.alignY = View.ALIGN_CENTER;
            busyText.x = 0.5f * busyView.width;
            busyText.y = 0.5f * busyView.height;
            busyView.AddView(busyText);

            AddView(busyView);

            View buttonGroup = new View();

            TextButton button = new TextButton("BACK", 0, 0, 100, 20);
            buttonGroup.AddView(button);

            button = new TextButton("REFRESH", 0, 0, 100, 20);
            buttonGroup.AddView(button);

            button = new TextButton("CREATE", 0, 0, 100, 20);
            buttonGroup.AddView(button);

            buttonGroup.LayoutHor(10);
            buttonGroup.ResizeToFitViews();

            buttonGroup.alignX = View.ALIGN_CENTER;
            buttonGroup.alignY = View.ALIGN_MAX;

            buttonGroup.x = 0.5f * width;
            buttonGroup.y = 432;

            AddView(buttonGroup);
        }

        public void SetBusy()
        {

        }

        public void SetEntries()
        {

        }
    }

    class ServerView : View
    {
        public ServerView(String name, int playersCount, int totalCount)
            : base(154, 143)
        {
            Font font = Helper.GetFont(A.fnt_button);

            TextView nameView = new TextView(font, name);

        }
    }

    class ServerMapView : View
    {
        int cw;
        int ch;

        public ServerMapView(int cw, int ch)
            : base(127, 86)
        {
            this.cw = cw;
            this.ch = ch;
        }

        public override void Draw(Context context)
        {
            PreDraw(context);
            PostDraw(context);
        }
    }
}
