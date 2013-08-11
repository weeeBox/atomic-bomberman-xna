using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Networking;
using BomberEngine.Debugging;
using System.Net;
using BomberEngine.Core;
using Lidgren.Network;

namespace Bomberman.Networking
{
    public class NetworkNotifications
    {
        public static readonly String ClientConnected           = "ClientConnected";        // peer:Server, clientConnection:NetConnection, clientName:String
        public static readonly String ClientDisconnected        = "ClientDisconnected";     // peer:Server, clientConnection:NetConnection

        public static readonly String ConnectedToServer         = "ConnectedToServer";      // peer:Client, serverConnection:NetConnection
        public static readonly String DisconnectedFromServer    = "DisconnectedFromServer"; // peer:Client, serverConnection:NetConnection

        public static readonly String LocalClientDiscovered     = "LocalClientDiscovered";  // peer:Server, msg:NetOutgoingMessage
        public static readonly String LocalServerDiscovered     = "LocalServerDiscovered";  // peer:Client, msg:NetIncomingMessage
    }

    public class NetworkManager : IUpdatable
    {
        private Peer networkPeer;
        private LocalServersDiscovery serverDiscovery;

        public void Update(float delta)
        {
            if (networkPeer != null)
            {
                networkPeer.Update(delta);
            }
            if (serverDiscovery != null)
            {
                serverDiscovery.Update(delta);
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Local server discovery

        public void StartLocalServerDiscovery()
        {
            Debug.Assert(serverDiscovery == null);

            String appId = CVars.sv_appId.value;
            int port = CVars.sv_port.intValue;

            serverDiscovery = new LocalServersDiscovery(appId, port);
            serverDiscovery.Start();

            Log.i("Started local servers discovery...");
        }

        public void StopLocalServerDiscovery()
        {
            if (serverDiscovery != null)
            {
                serverDiscovery.Stop();
                serverDiscovery = null;

                Log.i("Stopped local servers discovery");
            }
        }

        public void StartListeningForServerDiscovery()
        {   
            GetServer().StartListeningDiscoveryRequests();
        }

        public void StopListeningForServerDiscovery()
        {
            GetServer().StopListeningDiscoveryRequests();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Net peer

        public void StartServer()
        {
            String appId = CVars.sv_appId.value;
            int port = CVars.sv_port.intValue;

            CreateServer(appId, port);
            Start();
        }

        public void StartClient(IPEndPoint remoteEndPoint)
        {
            String appId = CVars.sv_appId.value;
            int port = CVars.sv_port.intValue;

            CreateClient(appId, remoteEndPoint, port);
            Start();
        }

        private void CreateServer(String appIdentifier, int port)
        {
            Debug.Assert(networkPeer == null);
            networkPeer = new Server(appIdentifier, port); ;

            Log.d("Created network server");
        }

        private void CreateClient(String appIdetifier, IPEndPoint remoteEndPoint, int port)
        {
            Debug.Assert(networkPeer == null);
            networkPeer = new Client(appIdetifier, remoteEndPoint);

            Log.d("Created network client");
        }

        private void Start()
        {
            Debug.Assert(networkPeer != null);
            networkPeer.Start();

            Log.d("Started network peer");
        }

        public void Stop()
        {
            StopLocalServerDiscovery();

            if (networkPeer != null)
            {
                networkPeer.Stop();
                networkPeer = null;

                Log.d("Stopped network peer");
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public NetOutgoingMessage CreateMessage()
        {
            return networkPeer.CreateMessage();
        }

        public void SendMessage(NetOutgoingMessage message, NetConnection recipient)
        {
            networkPeer.SendMessage(message, recipient);
        }

        public void SendMessage(NetOutgoingMessage message)
        {
            networkPeer.SendMessage(message);
        }

        public void RecycleMessage(NetOutgoingMessage msg)
        {
            networkPeer.RecycleMessage(msg);
        }

        public void RecycleMessage(NetIncomingMessage msg)
        {
            networkPeer.RecycleMessage(msg);
        }

        //////////////////////////////////////////////////////////////////////////////

        internal Peer GetPeer()
        {
            return networkPeer;
        }

        internal Server GetServer()
        {
            Server server = networkPeer as Server;
            Debug.Assert(server != null);

            return server;
        }

        internal Client GetClient()
        {
            Client client = networkPeer as Client;
            Debug.Assert(client != null);

            return client;
        }
    }
}
