using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using Bomberman.Networking;
using BomberEngine.Debugging;
using Bomberman.Game.Screens;
using BomberEngine.Core;
using BomberEngine.Core.Visual;
using Bomberman.Content;
using Lidgren.Network;
using BombermanCommon.Resources.Scheme;
using System.Net;

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

    public class MultiplayerController : BombermanController, ClientListener, ServerListener, LocalServersDiscoveryListener
    {
        public enum ExitCode
        {
            Cancel,
            Create,
            Join,
        }

        public enum ConnectingId
        {
            ConnectToServer
        }

        private LocalServersDiscovery serverDiscovery;
        private List<ServerInfo> foundServers;

        private MultiplayerScreen multiplayerScreen;
        private ServerInfo serverInfo;

        private NetworkConnectionScreen connectionScreen;

        protected override void OnStart()
        {
            multiplayerScreen = new MultiplayerScreen(OnButtonPressed, false);
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
            Debug.Assert(serverDiscovery == null);

            String name = CVars.sv_appId.value;
            int port = CVars.sv_port.intValue;

            serverDiscovery = new LocalServersDiscovery(this, name, port);
            serverDiscovery.Start();

            foundServers = new List<ServerInfo>();

            multiplayerScreen.AddUpdatable(UpdateDiscovery);
            multiplayerScreen.ScheduleCall(StopDiscoveryCall, 5.0f);

            Log.i("Started local servers discovery...");
            multiplayerScreen.SetBusy();
        }

        private void StopDiscovery(bool updateUI)
        {
            if (serverDiscovery != null)
            {
                serverDiscovery.Stop();
                serverDiscovery = null;

                multiplayerScreen.RemoveUpdatable(UpdateDiscovery);

                if (updateUI)
                {
                    multiplayerScreen.SetServers(foundServers);
                }

                Log.i("Stopped local servers discovery");
            }
        }

        private void StopDiscoveryCall(DelayedCall call)
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
            GetRootController().StartGameServer(this);
        }

        private void StartClient(IPEndPoint remoteEndPoint)
        {   
            GetRootController().StartGameClient(remoteEndPoint, this);
        }

        private void StopPeer()
        {
            serverInfo = null;
            GetRootController().StopPeer();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region ClientListener

        public void OnMessageReceived(Client client, NetworkMessageId messageId, NetIncomingMessage message)
        {
            throw new NotImplementedException();
        }

        public void OnConnectedToServer(Client client, NetConnection serverConnection)
        {
            throw new NotImplementedException();
        }

        public void OnDisconnectedFromServer(Client client)
        {
            throw new NotImplementedException();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region ServerListener

        public void OnMessageReceived(Server server, NetworkMessageId messageId, NetIncomingMessage message)
        {
            throw new NotImplementedException();
        }

        public void OnClientConnected(Server server, NetConnection connection)
        {
            throw new NotImplementedException();
        }

        public void OnClientDisconnected(Server server, NetConnection connection)
        {
            throw new NotImplementedException();
        }

        public void OnDiscoveryResponse(Server server, NetOutgoingMessage msg)
        {
            Debug.Assert(serverInfo != null);
            WriteServerInfo(msg, serverInfo);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region LocalServersDiscoveryListener

        public void OnServerDiscoveryResponse(LocalServersDiscovery serverDiscovery, NetIncomingMessage msg)
        {
            ServerInfo info = ReadServerInfo(msg);
            OnLocalServerFound(info);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Server Info

        private static ServerInfo ReadServerInfo(NetIncomingMessage message)
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

        private static void WriteServerInfo(NetOutgoingMessage message, ServerInfo info)
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
                    Stop(ExitCode.Cancel);
                    break;
                }

                case MultiplayerScreen.ButtonId.Create:
                {
                    StopDiscovery(false);

                    String hostName = CVars.sv_hostname.value;

                    Scheme scheme = Application.Assets().LoadAsset<Scheme>("Content\\maps\\x.sch");
                    serverInfo = new ServerInfo(hostName);
                    serverInfo.scheme = scheme;
                    StartNextScreen(new MultiplayerLobbyScreen(serverInfo, OnLobbyScreenButtonPressed));

                    StartServer();
                    break;
                }

                case MultiplayerScreen.ButtonId.Join:
                {
                    ServerInfo info = button.data as ServerInfo;
                    Debug.Assert(info != null);

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

        private void StartConnectionScreen(ConnectingId connectionId)
        {
            if (connectionScreen == null)
            {
                connectionScreen = new NetworkConnectionScreen(OnConnectionCancelled);
            }

            connectionScreen.operationId = (int)connectionId;
            StartNextScreen(connectionScreen);
        }

        private void OnConnectionCancelled(int operationId)
        {
            ConnectingId connectingId = (ConnectingId)operationId;

        }

        #endregion
    }
}
