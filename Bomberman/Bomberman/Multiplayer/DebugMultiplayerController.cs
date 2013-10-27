using System;
using System.Net;
using Assets;
using BomberEngine;
using Bomberman.Content;
using Bomberman.Game;
using Bomberman.Networking;
using Bomberman.UI;
using Lidgren.Network;

namespace Bomberman.Multiplayer
{
    public class DebugMultiplayerController : BmController
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

            GetNetwork().StartLocalServerDiscovery();
            Application.ScheduleTimer(OnDiscoveryTimeout, 1.0f);
        }

        private void StopDiscovery()
        {
            UnregisterNotification(NetworkNotifications.LocalServerDiscovered, LocalServerDiscoveredNotification);

            GetNetwork().StopLocalServerDiscovery();
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

            GetNetwork().StartServer();
            GetNetwork().StartListeningForServerDiscovery();
        }

        private void StartClient(IPEndPoint remoteEndPoint)
        {
            GetNetwork().StartClient(remoteEndPoint);
        }

        private void StopPeer()
        {
            serverInfo = null;
            GetNetwork().Stop();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Notifications

        public void ClientConnectedNotification(Notification notification)
        {
            Stop(ExitCode.ServerStarted, serverInfo);
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
