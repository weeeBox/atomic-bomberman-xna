using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Assets.Types;
using Assets;
using Bomberman.Network;
using Bomberman.Game;

namespace Bomberman.Multiplayer
{
    public class GameLobbyScreen : Screen
    {
        public enum ButtonId
        {
            Back,
            Refresh,
            Create,
            Join,
        }

        private bool local;
        private View contentView;
        private View busyView;

        public GameLobbyScreen(ButtonDelegate buttonDelegate, bool local)
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
            button.SetDelegate(buttonDelegate);
            button.id = (int)ButtonId.Back;
            buttonGroup.AddView(button);

            button = new TextButton("REFRESH", 0, 0, 100, 20);
            button.SetDelegate(buttonDelegate);
            button.id = (int)ButtonId.Refresh;
            buttonGroup.AddView(button);

            button = new TextButton("CREATE", 0, 0, 100, 20);
            button.SetDelegate(buttonDelegate);
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
                    ServerInfo server = servers[i];
                    ServerView view = new ServerView(server);
                    contentView.AddView(view);
                }
            }
            else
            {
                Font font = Helper.GetFont(A.fnt_button);

                TextView text = new TextView(font, "No servers found.");
                text.alignX = text.alignY = View.ALIGN_CENTER;
                text.x = 0.5f * contentView.width;
                text.y = 0.5f * contentView.height;
                contentView.AddView(text);
            }
        }
    }

    class ServerView : View
    {
        public ServerView(ServerInfo server)
            : base(154, 143)
        {
            Font font = Helper.GetFont(A.fnt_button);

            TextView nameView = new TextView(font, server.name);
            nameView.alignX = View.ALIGN_CENTER;
            nameView.x = 0.5f * width;
            nameView.y = 104;
            AddView(nameView);
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
