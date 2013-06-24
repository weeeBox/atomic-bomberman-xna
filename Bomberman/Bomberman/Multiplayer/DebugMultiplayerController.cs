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
using BomberEngine.Core.Assets.Types;
using Bomberman.Game;
using Assets;
using BomberEngine.Core;

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

            Font font = Helper.GetFont(A.fnt_system);
            TextView textView = new TextView(font, mode.ToString());
            textView.alignX = View.ALIGN_CENTER;
            textView.x = 0.5f * screen.width;
            textView.y = 10;
            screen.AddView(textView);

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
            GetMultiplayerManager().StartLocalServerDiscovery(this);
            Application.ScheduleCall(OnDiscoveryTimeout, 1.0f);
        }

        private void StopDiscovery()
        {
            GetMultiplayerManager().StopLocalServerDiscovery();
            Application.CancelCall(OnDiscoveryTimeout);
        }

        private void OnDiscoveryTimeout(DelayedCall call)
        {
            Log.d("Discovery timeout");

            StopDiscovery();
            StartDiscovery();
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

            GetMultiplayerManager().StartServer(this);
            GetMultiplayerManager().StartListeningForServerDiscovery(this);
        }

        private void StartClient(IPEndPoint remoteEndPoint)
        {
            GetMultiplayerManager().StartClient(remoteEndPoint, this);
        }

        private void StopPeer()
        {
            serverInfo = null;
            GetMultiplayerManager().Stop();
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
