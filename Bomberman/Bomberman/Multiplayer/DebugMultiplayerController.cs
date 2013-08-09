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
using Bomberman.UI;
using BomberEngine.Core.Events;

namespace Bomberman.Multiplayer
{
    public class DebugMultiplayerController : BmController, IServerListener, IClientListener
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
                RegisterNotification(NetworkNotifications.ConnectedToServer, ConnectedToServerNotification);
                StartDiscovery();
            }
            else if (mode == Mode.Server)
            {
                RegisterNotification(NetworkNotifications.ClientConnected, ClientConnectedNotification);
                RegisterNotification(NetworkNotifications.LocalClientDiscovered, LocalClientDiscoveredNotification);
                StartServer();
            }

            Screen screen = new Screen();
            Button button = new TempButton("Cancel");
            button.id = (int)ButtonId.Cancel;
            button.buttonDelegate = OnCancelButtonPressed;
            button.alignX = button.alignY = button.parentAlignX = button.parentAlignY = View.ALIGN_CENTER;
            screen.AddView(button);
            screen.SetCancelButton(button);

            Font font = Helper.fontSystem;
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
            RegisterNotification(NetworkNotifications.LocalServerDiscovered, LocalServerDiscoveredNotification);

            GetMultiplayerManager().StartLocalServerDiscovery();
            Application.ScheduleTimer(OnDiscoveryTimeout, 1.0f);
        }

        private void StopDiscovery()
        {
            UnregisterNotification(NetworkNotifications.LocalServerDiscovered, LocalServerDiscoveredNotification);

            GetMultiplayerManager().StopLocalServerDiscovery();
            Application.CancelTimer(OnDiscoveryTimeout);
        }

        private void OnDiscoveryTimeout(Timer call)
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

            Scheme scheme = BmApplication.Assets().GetScheme(A.maps_x);
            serverInfo = new ServerInfo(hostName);
            serverInfo.scheme = scheme;

            GetMultiplayerManager().StartServer(this);
            GetMultiplayerManager().StartListeningForServerDiscovery();
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

        public void ClientConnectedNotification(Notification notification)
        {   
            Stop(ExitCode.ServerStarted, serverInfo);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Client listener

        public void OnMessageReceived(Client client, NetworkMessageId messageId, Lidgren.Network.NetIncomingMessage message)
        {
        }

        public void ConnectedToServerNotification(Notification notification)
        {
            Stop(ExitCode.ClientStarted, serverInfo);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        private void LocalClientDiscoveredNotification(Notification notification)
        {
            NetOutgoingMessage msg = notification.GetNotNullData<NetOutgoingMessage>();
            MultiplayerController.WriteServerInfo(msg, serverInfo);
        }

        private void LocalServerDiscoveredNotification(Notification notification)
        {
            NetIncomingMessage msg = notification.GetNotNullData<NetIncomingMessage>();
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
