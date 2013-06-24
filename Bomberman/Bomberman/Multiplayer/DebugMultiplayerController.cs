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
using Lidgren.Network;

namespace Bomberman.Multiplayer
{
    public class DebugMultiplayerController : BombermanController, 
        ILocalServersDiscoveryRequestListener, ILocalServersDiscoveryResponseListener,
        ServerListener, ClientListener
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

        public enum ExitCode
        {
            Cancel,
            ClientStarted,
            ServerStarted,
        }

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
            button.SetDelegate(OnCancelButtonPressed);
            button.alignX = button.alignY = button.parentAlignX = button.parentAlignY = View.ALIGN_CENTER;
            screen.AddView(button);
            screen.SetBackButton(button);

            StartScreen(screen);
        }

        protected override void OnStop()
        {
            StopDiscovery();
        }

        //////////////////////////////////////////////////////////////////////////////

        private void OnCancelButtonPressed(Button button)
        {
            StopPeer();
            Stop(ExitCode.Cancel);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Local server discovery

        private void StartDiscovery()
        {
            GetRootController().StartLocalServerDiscovery(this);
        }

        private void StopDiscovery()
        {
            GetRootController().StopLocalServerDiscovery();
        }

        private void OnLocalServerFound(ServerInfo info)
        {
            Log.d("Found local server: " + info.endPoint);
            StopDiscovery();

            serverInfo = info;
            StartClient(info.endPoint);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Peer

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

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Server listener

        public void OnMessageReceived(Server server, NetworkMessageId messageId, NetIncomingMessage message)
        {
        }

        public void OnClientConnected(Server server, string name, Lidgren.Network.NetConnection connection)
        {
            Log.d("Client connected: name=" + name + " endpoint=" + connection.RemoteEndPoint);
            Stop(ExitCode.ServerStarted, serverInfo);
        }

        public void OnClientDisconnected(Server server, Lidgren.Network.NetConnection connection)
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Client listener

        public void OnMessageReceived(Client client, NetworkMessageId messageId, Lidgren.Network.NetIncomingMessage message)
        {
        }

        public void OnConnectedToServer(Client client, Lidgren.Network.NetConnection serverConnection)
        {
            Stop(ExitCode.ClientStarted, serverInfo);
        }

        public void OnDisconnectedFromServer(Client client)
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public void OnServerDiscoveryRequest(NetOutgoingMessage msg)
        {
            MultiplayerController.WriteServerInfo(msg, serverInfo);
        }

        public void OnServerDiscoveryResponse(NetIncomingMessage msg)
        {
            ServerInfo info = MultiplayerController.ReadServerInfo(msg);
            OnLocalServerFound(info);
        }

        //////////////////////////////////////////////////////////////////////////////

        private void Stop(ExitCode code, ServerInfo serverInfo = null)
        {
            Stop((int)code, serverInfo);
        }
    }
}
