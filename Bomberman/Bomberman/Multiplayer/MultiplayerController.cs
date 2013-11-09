using System;
using System.Collections.Generic;
using System.Net;
using BomberEngine;
using Bomberman.Common.Popups;
using Bomberman.Content;
using Bomberman.Networking;
using BombermanCommon.Resources.Scheme;
using Lidgren.Network;

namespace Bomberman.Multiplayer
{
    public class ServerInfo
    {
        public String name;
        public IPEndPoint endPoint;

        public Scheme scheme;

        public ServerInfo(String name)
        {
            this.name = name;
        }

        public ServerInfo(String name, IPEndPoint endPoint)
        {
            this.name = name;
            this.endPoint = endPoint;
        }
    }

    public class MultiplayerController : BmController
    {
        public enum ScreenId
        {
            Multiplyer,
            Lobby
        }

        public enum ExitCode
        {
            Create,
            Join,
        }

        private LocalServersDiscovery serverDiscovery;
        private List<ServerInfo> foundServers;
        private ServerInfo serverInfo;

        private NetworkConnectionScreen connectionScreen;

        protected override void OnStart()
        {
            Screen multiplayerScreen = new MultiplayerScreen(OnButtonPressed);
            multiplayerScreen.id = (int)ScreenId.Multiplyer;
            StartScreen(multiplayerScreen);

            StartDiscovery();
        }

        protected override void OnStop()
        {
            StopDiscovery(false);
        }

        private void Stop(ExitCode exitCode, Object data = null)
        {
            Stop((int)exitCode, data);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Local server discovery

        private void UpdateDiscovery(float delta)
        {
            serverDiscovery.Update(delta);
        }

        private void StartDiscovery()
        {
            //Assert.True(serverDiscovery == null);

            //String name = CVars.sv_appId.value;
            //int port = CVars.sv_port.intValue;

            //serverDiscovery = new LocalServersDiscovery(this, name, port);
            //serverDiscovery.Start();

            //foundServers = new List<ServerInfo>();

            //MultiplayerScreen multiplayerScreen = (MultiplayerScreen)FindScreen(ScreenId.Multiplyer);
            //multiplayerScreen.AddUpdatable(UpdateDiscovery);
            //multiplayerScreen.ScheduleCall(StopDiscoveryCall, 5.0f);

            //Log.i("Started local servers discovery...");
            //multiplayerScreen.SetBusy();

            throw new NotImplementedException();
        }

        private void StopDiscovery(bool updateUI)
        {
            //if (serverDiscovery != null)
            //{
            //    serverDiscovery.Stop();
            //    serverDiscovery = null;

            //    MultiplayerScreen multiplayerScreen = (MultiplayerScreen)FindScreen(ScreenId.Multiplyer);
            //    if (multiplayerScreen != null)
            //    {
            //        multiplayerScreen.RemoveUpdatable(UpdateDiscovery);

            //        if (updateUI)
            //        {
            //            multiplayerScreen.SetServers(foundServers);
            //        }
            //    }

            //    Log.i("Stopped local servers discovery");
            //}

            throw new NotImplementedException();
        }

        private void StopDiscoveryCall(Timer call)
        {
            StopDiscovery(true);
        }

        private void OnLocalServerFound(ServerInfo info)
        {
            Log.d("Found local server: " + info.endPoint);
            foundServers.Add(info);
        }

        #endregion

        private void StartServer()
        {   
            GetNetwork().StartServer();
        }
        
        private void StartClient(IPEndPoint remoteEndPoint)
        {
            StartConnectionScreen(OnServerConnectionCancelled, "Connecting to " + remoteEndPoint + "...");
            GetNetwork().StartClient(remoteEndPoint);
        }

        private void StopPeer()
        {
            serverInfo = null;
            GetNetwork().Stop();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region ClientListener

        public void OnConnectedToServer(Client client, NetConnection serverConnection)
        {
            StopConnectionScreen();
            StartLobbyScreen(serverInfo, false);
        }

        public void OnDisconnectedFromServer(Client client)
        {
            Screen currentScreen = CurrentScreen();
            ScreenId screenId = (ScreenId)currentScreen.id;

            switch (screenId)
            {
                case ScreenId.Lobby:
                    StopPeer();
                    DialogPopup.ShowMessage(OnDisconnectedFromServerDialogFinish, "Server connection lost");
                    break;
                case ScreenId.Multiplyer:
                    break;
            }
        }

        private void OnDisconnectedFromServerDialogFinish(DialogPopup popup, int buttonId)
        {
            MultiplayerLobbyScreen lobbyScreen = FindLobbyScreen();
            Assert.True(lobbyScreen != null);

            lobbyScreen.Finish();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region ServerListener

        public void OnClientConnected(Server server, String name, NetConnection connection)
        {
            MultiplayerLobbyScreen lobbyScreen = FindLobbyScreen();
            lobbyScreen.AddClient(name, connection);
        }

        public void OnClientDisconnected(Server server, NetConnection connection)
        {
            MultiplayerLobbyScreen lobbyScreen = FindLobbyScreen();
            lobbyScreen.RemoveClient(connection);
        }

        public void OnDiscoveryResponse(Server server, NetOutgoingMessage msg)
        {
            Assert.True(serverInfo != null);
            WriteServerInfo(msg, serverInfo);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Notifications

        private void LocalServerDiscoveredNotification(Notification notification)
        {
            NetIncomingMessage msg = notification.GetData<NetIncomingMessage>();
            
            ServerInfo info = ReadServerInfo(msg);
            OnLocalServerFound(info);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Server Info

        internal static ServerInfo ReadServerInfo(NetIncomingMessage message)
        {
            // name
            String name = message.ReadString();

            // scheme
            Scheme scheme = new Scheme();

            // scheme: name
            scheme.name = message.ReadString();

            // scheme: field data
            int fieldWidth = message.ReadInt32();
            int fieldHeight = message.ReadInt32();
            FieldBlocks[] fieldDataArray = new FieldBlocks[fieldWidth * fieldHeight];
            for (int i = 0; i < fieldDataArray.Length; ++i)
            {
                fieldDataArray[i] = (FieldBlocks)message.ReadByte();
            }
            scheme.fieldData = new FieldData(fieldWidth, fieldHeight, fieldDataArray);

            // scheme: player locations
            int locationsCount = message.ReadByte();
            PlayerLocationInfo[] playerLocations = new PlayerLocationInfo[locationsCount];
            for (int i = 0; i < locationsCount; ++i)
            {
                int x = message.ReadByte();
                int y = message.ReadByte();
                int team = message.ReadByte();

                playerLocations[i] = new PlayerLocationInfo(i, x, y, team);
            }
            scheme.playerLocations = playerLocations;

            ServerInfo info = new ServerInfo(name, message.SenderEndPoint);
            info.scheme = scheme;
            return info;
        }

        internal static void WriteServerInfo(NetOutgoingMessage message, ServerInfo info)
        {
            // name
            message.Write(info.name);

            // scheme
            Scheme scheme = info.scheme;

            // scheme: name
            message.Write(scheme.name);

            // scheme: field data
            FieldData fieldData = scheme.fieldData;
            message.Write(fieldData.GetWidth());
            message.Write(fieldData.GetHeight());

            FieldBlocks[] blocks = fieldData.GetDataArray();
            for (int i = 0; i < blocks.Length; ++i)
            {
                byte block = (byte)blocks[i];
                message.Write(block);
            }

            // scheme: player locations
            PlayerLocationInfo[] playerLocations = scheme.GetPlayerLocations();
            message.Write((byte)playerLocations.Length);
            for (int i = 0; i < playerLocations.Length; ++i)
            {
                message.Write((byte)playerLocations[i].x);
                message.Write((byte)playerLocations[i].y);
                message.Write((byte)playerLocations[i].team);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Button delegate

        private void OnButtonPressed(Button button)
        {
            MultiplayerScreen.ButtonId buttonId = (MultiplayerScreen.ButtonId)button.id;
            switch (buttonId)
            {
                case MultiplayerScreen.ButtonId.Back:
                {
                    Stop();
                    break;
                }

                case MultiplayerScreen.ButtonId.Create:
                {
                    StopDiscovery(false);

                    String hostName = CVars.sv_hostname.value;

                    Scheme scheme = Application.Assets().LoadAsset<Scheme>("Content\\maps\\x.sch");
                    serverInfo = new ServerInfo(hostName);
                    serverInfo.scheme = scheme;
                    StartLobbyScreen(serverInfo, true);

                    StartServer();
                    break;
                }

                case MultiplayerScreen.ButtonId.Join:
                {
                    ServerInfo info = button.data as ServerInfo;
                    Assert.True(info != null);

                    serverInfo = info;
                    StartClient(info.endPoint);
                    break;
                }

                case MultiplayerScreen.ButtonId.Refresh:
                {
                    StopDiscovery(false);
                    StartDiscovery();
                    break;
                }
            }
        }

        private void OnLobbyScreenButtonPressed(Button button)
        {
            MultiplayerLobbyScreen.ButtonId buttonId = (MultiplayerLobbyScreen.ButtonId)button.id;
            switch (buttonId)
            {
                case MultiplayerLobbyScreen.ButtonId.Start:
                {   
                    break;
                }

                case MultiplayerLobbyScreen.ButtonId.Back:
                {
                    StopPeer();
                    break;
                }
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Connecting screen

        private void StartConnectionScreen(ConnectionCancelCallback cancelCallback, String message)
        {
            if (connectionScreen == null)
            {
                connectionScreen = new NetworkConnectionScreen();
            }

            connectionScreen.cancelCallback = cancelCallback;
            connectionScreen.SetStatusText(message);

            if (CurrentScreen() != connectionScreen)
            {
                StartNextScreen(connectionScreen);
            }
        }

        private void StopConnectionScreen()
        {
            if (CurrentScreen() == connectionScreen)
            {
                connectionScreen.Finish();
            }
        }

        private void OnServerConnectionCancelled()
        {
            Log.d("Cancelled connecting to the server");
            StopPeer();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        private void StartLobbyScreen(ServerInfo info, bool isServer)
        {
            Screen screen = new MultiplayerLobbyScreen(serverInfo, OnLobbyScreenButtonPressed, isServer);
            screen.id = (int)ScreenId.Lobby;
            StartNextScreen(screen);
        }

        private MultiplayerLobbyScreen FindLobbyScreen()
        {
            return (MultiplayerLobbyScreen)FindScreen(ScreenId.Lobby);
        }

        private Screen FindScreen(ScreenId id)
        {
            return FindScreen((int)id);
        }
    }
}
