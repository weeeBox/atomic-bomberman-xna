using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Networking;
using BomberEngine.Debugging;
using System.Net;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using Bomberman.Content;

namespace Bomberman.Multiplayer
{
    public class DebugMultiplayerController : BombermanController, LocalServersDiscoveryListener, ServerListener, ClientListener
    {
        public enum Mode
        {
            Client,
            Server
        }

        public enum ButtonId
        {
            Cancel
        }

        private LocalServersDiscovery serverDiscovery;
        private Mode mode;
        private ServerInfo serverInfo;

        public DebugMultiplayerController(Mode mode)
        {
            this.mode = mode;
        }

        protected override void OnStart()
        {
            if (mode == Mode.Client)
            {
                StartDiscovery();
            }
            else if (mode == Mode.Server)
            {
                StartServer();
            }

            Screen screen = new Screen();
            Button button = new TextButton("Cancel", 0, 0, 100, 20);
            button.id = (int)ButtonId.Cancel;
            button.SetDelegate(OnButtonPressed);
            button.alignX = button.alignY = button.parentAlignX = button.parentAlignY = View.ALIGN_CENTER;
            screen.AddView(button);
            screen.SetBackButton(button);

            StartScreen(screen);
        }

        protected override void OnStop()
        {
            StopDiscovery();
            StopPeer();
        }

        //////////////////////////////////////////////////////////////////////////////

        private void OnButtonPressed(Button button)
        {
            Stop();
        }

        //////////////////////////////////////////////////////////////////////////////

        public override void Update(float delta)
        {
            base.Update(delta);

            if (serverDiscovery != null)
            {
                serverDiscovery.Update(delta);
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Local server discovery

        private void StartDiscovery()
        {
            Debug.Assert(serverDiscovery == null);

            String name = CVars.sv_appId.value;
            int port = CVars.sv_port.intValue;

            serverDiscovery = new LocalServersDiscovery(this, name, port);
            serverDiscovery.Start();

            Log.i("Started local servers discovery...");
        }

        private void StopDiscovery()
        {
            if (serverDiscovery != null)
            {
                serverDiscovery.Stop();
                serverDiscovery = null;

                Log.i("Stopped local servers discovery");
            }
        }

        private void OnLocalServerFound(ServerInfo info)
        {
            Log.d("Found local server: " + info.endPoint);
            StopDiscovery();

            StartClient(info.endPoint);
        }

        #endregion

        private void StartServer()
        {
            String hostName = CVars.sv_hostname.value;

            Scheme scheme = Application.Assets().LoadAsset<Scheme>("Content\\maps\\x.sch");
            serverInfo = new ServerInfo(hostName);
            serverInfo.scheme = scheme;

            GetRootController().StartGameServer(this);
        }

        private void StartClient(IPEndPoint remoteEndPoint)
        {   
            GetRootController().StartGameClient(remoteEndPoint, this);
        }

        private void StopPeer()
        {   
            GetRootController().StopPeer();
        }

        public void OnMessageReceived(Server server, NetworkMessageId messageId, Lidgren.Network.NetIncomingMessage message)
        {   
        }

        public void OnClientConnected(Server server, string name, Lidgren.Network.NetConnection connection)
        {
            Log.d("Client connected: name=" + name + " endpoint=" + connection.RemoteEndPoint);
        }

        public void OnClientDisconnected(Server server, Lidgren.Network.NetConnection connection)
        {
        }

        public void OnDiscoveryResponse(Server server, Lidgren.Network.NetOutgoingMessage message)
        {
            MultiplayerController.WriteServerInfo(message, serverInfo);
        }

        public void OnMessageReceived(Client client, NetworkMessageId messageId, Lidgren.Network.NetIncomingMessage message)
        {
        }

        public void OnConnectedToServer(Client client, Lidgren.Network.NetConnection serverConnection)
        {
            Log.d("Connected to the server: " + serverConnection.RemoteEndPoint);
        }

        public void OnDisconnectedFromServer(Client client)
        {
        }

        public void OnServerDiscoveryResponse(LocalServersDiscovery serverDiscovery, Lidgren.Network.NetIncomingMessage msg)
        {
            ServerInfo info = MultiplayerController.ReadServerInfo(msg);
            OnLocalServerFound(info);
        }
    }
}
