using System;
using System.Collections.Generic;
using BomberEngine;
using Bomberman.Game;
using Bomberman.UI;
using Lidgren.Network;

namespace Bomberman.Multiplayer
{
    public class MultiplayerLobbyScreen : Screen
    {
        public enum ButtonId
        {
            Back,
            Start,
            Ready,
        }

        private View clientsView;
        private ServerInfo serverInfo;
        private bool isServer;
        private IDictionary<NetConnection, ClientInfoView> viewsLookup;

        public MultiplayerLobbyScreen(ServerInfo serverInfo, ButtonDelegate buttonDelegate, bool isServer)
        {
            this.serverInfo = serverInfo;
            viewsLookup = new Dictionary<NetConnection, ClientInfoView>();

            View contentView = new View(512, 363);
            contentView.alignX = View.ALIGN_CENTER;
            contentView.x = 0.5f * width;
            contentView.y = 48;

            Font font = GetDefaultFont();

            contentView.AddView(new View(215, 145));

            TextView serverName = new TextView(font, serverInfo.name);
            serverName.alignX = View.ALIGN_CENTER;
            serverName.x = 113;
            serverName.y = 155;

            contentView.AddView(serverName);

            clientsView = new View(286, contentView.height);
            clientsView.alignX = View.ALIGN_MAX;
            clientsView.x = contentView.width;
            contentView.AddView(clientsView);

            AddView(contentView);

            View buttons = new View();
            buttons.x = 0.5f * width;
            buttons.y = 432;
            buttons.alignX = View.ALIGN_CENTER;
            buttons.alignY = View.ALIGN_MAX;

            Button button = new TempButton("BACK");
            button.buttonDelegate = buttonDelegate;
            button.id = (int)ButtonId.Back;
            buttons.AddView(button);
            SetCancelButton(button);

            String label = isServer ? "START" : "READY";
            ButtonId buttonId = isServer ? ButtonId.Start : ButtonId.Ready;

            button = new TempButton(label);
            button.buttonDelegate = buttonDelegate;
            button.id = (int)buttonId;
            buttons.AddView(button);

            buttons.LayoutHor(10);
            buttons.ResizeToFitViews();
            AddView(buttons);
        }

        public void AddClient(String name, NetConnection connection)
        {
            Debug.Assert(FindClientView(connection) == null);

            ClientInfoView clientView = new ClientInfoView(name, connection);
            clientsView.AddView(clientView);
            clientsView.LayoutVer(10);

            viewsLookup[connection] = clientView;
        }

        public void RemoveClient(NetConnection connection)
        {
            ClientInfoView clientView = FindClientView(connection);
            Debug.Assert(clientView != null);

            viewsLookup.Remove(connection);

            clientsView.RemoveView(clientView);
            clientsView.LayoutVer(10);
        }

        private ClientInfoView FindClientView(NetConnection connection)
        {
            ClientInfoView view;
            if (viewsLookup.TryGetValue(connection, out view))
            {
                return view;
            }
            return null;
        }

        private static Font GetDefaultFont()
        {
            return Helper.fontButton;
        }
    }

    class ClientInfoView : View
    {
        public ClientInfoView(String name, NetConnection connection)
        {
            Font font = Helper.fontButton;

            TextView textView = new TextView(font, connection.RemoteEndPoint + ": " + name);
            AddView(textView);

            ResizeToFitViews();
        }
    }
}
