using System.Collections.Generic;
using BomberEngine;
using Bomberman.Gameplay;
using Bomberman.UI;
using Microsoft.Xna.Framework;

namespace Bomberman.Multiplayer
{
    public class MultiplayerScreen : Screen
    {
        public enum ButtonId
        {
            Back,
            Refresh,
            Create,
            Join,
        }

        private View contentView;
        private View busyView;
        private ButtonDelegate buttonDelegate;

        public MultiplayerScreen(ButtonDelegate buttonDelegate)
        {
            this.buttonDelegate = buttonDelegate;

            Font font = Helper.fontButton;

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

            Button button = new TempButton("BACK");
            button.buttonDelegate = buttonDelegate;
            button.id = (int)ButtonId.Back;
            buttonGroup.AddView(button);
            SetCancelButton(button);

            button = new TempButton("REFRESH");
            button.buttonDelegate = buttonDelegate;
            button.id = (int)ButtonId.Refresh;
            buttonGroup.AddView(button);

            button = new TempButton("CREATE");
            button.buttonDelegate = buttonDelegate;
            button.id = (int)ButtonId.Create;
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
            busyView.visible = true;
            contentView.visible = false;
        }

        public void SetServers(List<ServerInfo> servers)
        {
            busyView.visible = false;
            contentView.visible = true;

            contentView.RemoveViews();

            if (servers.Count > 0)
            {
                for (int i = 0; i < servers.Count; ++i)
                {
                    ServerInfo serverInfo = servers[i];
                    ServerView view = new ServerView(serverInfo);
                    view.buttonDelegate = buttonDelegate;
                    view.id = (int)ButtonId.Join;
                    view.data = serverInfo;
                    contentView.AddView(view);
                }
            }
            else
            {
                Font font = Helper.fontButton;

                TextView text = new TextView(font, "No servers found.");
                text.alignX = text.alignY = View.ALIGN_CENTER;
                text.x = 0.5f * contentView.width;
                text.y = 0.5f * contentView.height;
                contentView.AddView(text);
            }
        }
    }

    class ServerView : Button
    {
        private TextView nameView;

        public ServerView(ServerInfo server)
            : base(154, 143)
        {
            Font font = Helper.fontButton;

            nameView = new TextView(font, server.name);
            nameView.alignX = View.ALIGN_CENTER;
            nameView.x = 0.5f * width;
            nameView.y = 104;
            AddView(nameView);
        }

        protected override void OnFocusChanged(bool focused)
        {
            nameView.color = focused ? Color.Yellow : Color.White;
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
