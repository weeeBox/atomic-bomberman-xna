using System;
using System.Net;
using BomberEngine;
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
        private Peer m_networkPeer;
        private LocalServersDiscovery m_serverDiscovery;

        public void Update(float delta)
        {
            if (m_networkPeer != null)
            {
                m_networkPeer.Update(delta);
            }
            if (m_serverDiscovery != null)
            {
                m_serverDiscovery.Update(delta);
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Local server discovery

        public void StartLocalServerDiscovery()
        {
            Assert.IsTrue(m_serverDiscovery == null);

            String appId = CVars.sv_appId.value;
            int port = CVars.sv_port.intValue;

            m_serverDiscovery = new LocalServersDiscovery(appId, port);
            m_serverDiscovery.Start();

            Log.i("Started local servers discovery...");
        }

        public void StopLocalServerDiscovery()
        {
            if (m_serverDiscovery != null)
            {
                m_serverDiscovery.Stop();
                m_serverDiscovery = null;

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
            Assert.IsTrue(m_networkPeer == null);
            m_networkPeer = new Server(appIdentifier, port); ;

            Log.d("Created network server");
        }

        private void CreateClient(String appIdetifier, IPEndPoint remoteEndPoint, int port)
        {
            Assert.IsTrue(m_networkPeer == null);
            m_networkPeer = new Client(appIdetifier, remoteEndPoint);

            Log.d("Created network client");
        }

        private void Start()
        {
            Assert.IsTrue(m_networkPeer != null);
            m_networkPeer.Start();

            Log.d("Started network peer");
        }

        public void Stop()
        {
            StopLocalServerDiscovery();

            if (m_networkPeer != null)
            {
                m_networkPeer.Stop();
                m_networkPeer = null;

                Log.d("Stopped network peer");
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public NetOutgoingMessage CreateMessage()
        {
            return m_networkPeer.CreateMessage();
        }

        public void SendMessage(NetOutgoingMessage message, NetConnection recipient)
        {
            m_networkPeer.SendMessage(message, recipient);
        }

        public void SendMessage(NetOutgoingMessage message)
        {
            m_networkPeer.SendMessage(message);
        }

        public void RecycleMessage(NetOutgoingMessage msg)
        {
            m_networkPeer.RecycleMessage(msg);
        }

        public void RecycleMessage(NetIncomingMessage msg)
        {
            m_networkPeer.RecycleMessage(msg);
        }

        //////////////////////////////////////////////////////////////////////////////

        internal Peer GetPeer()
        {
            return m_networkPeer;
        }

        internal Server GetServer()
        {
            Server server = m_networkPeer as Server;
            Assert.IsTrue(server != null);

            return server;
        }

        internal Client GetClient()
        {
            Client client = m_networkPeer as Client;
            Assert.IsTrue(client != null);

            return client;
        }

        #if UNIT_TESTING

        public Peer networkPeer
        {
            get { return m_networkPeer; }
            set { m_networkPeer = value; }
        }

        #endif
    }
}
