using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Networking;
using BomberEngine.Debugging;
using System.Net;
using BomberEngine.Core;
using Lidgren.Network;

namespace Bomberman.Multiplayer
{
    public class NetworkNotifications
    {
        public static readonly String ClientConnected = "ClientConnected";
        public static readonly String ClientDisconnected = "ClientDisconnected";

        public static readonly String ConnectedToServer = "ConnectedToServer";
        public static readonly String DisconnectedFromServer = "DisconnectedFromServer";

        public static readonly String LocalClientDiscovered = "LocalClientDiscovered";
        public static readonly String LocalServerDiscovered = "LocalServerDiscovered";
    }

    public class MultiplayerManager : IUpdatable, IServerListener
    {
        private Peer networkPeer;
        private LocalServersDiscovery serverDiscovery;

        private IServerListener serverListener;

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

        public void StartServer(IServerListener listener)
        {
            String appId = CVars.sv_appId.value;
            int port = CVars.sv_port.intValue;

            CreateServer(appId, port);
            SetServerListener(listener);
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
            Server server = new Server(appIdentifier, port);
            server.listener = this;
            networkPeer = server;

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

            serverListener = null;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public NetOutgoingMessage CreateMessage()
        {
            return networkPeer.CreateMessage();
        }

        public NetOutgoingMessage CreateMessage(NetworkMessageId messageId)
        {
            return networkPeer.CreateMessage(messageId);
        }

        public void SendMessage(NetOutgoingMessage message, NetConnection recipient, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
        {
            networkPeer.SendMessage(message, recipient, method);
        }

        public void SendMessage(NetworkMessageId messageId, NetConnection recipient, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
        {
            networkPeer.SendMessage(messageId, recipient, method);
        }

        public void SendMessage(NetOutgoingMessage message, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
        {
            networkPeer.SendMessage(message, method);
        }

        public void SendMessage(NetworkMessageId messageId, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
        {
            networkPeer.SendMessage(messageId, method);
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

        #region Server listener

        public void OnMessageReceived(Server server, NetworkMessageId messageId, NetIncomingMessage message)
        {
            if (serverListener != null)
            {
                serverListener.OnMessageReceived(server, messageId, message);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Network messages delegates

        public void AddServerMessageDelegate(NetworkMessageId messageId, ReceivedMessageDelegate<Client> del)
        {
            GetClient().AddMessageDelegate(messageId, del);
        }

        public void RemoveServerMessageDelegate(NetworkMessageId messageId, ReceivedMessageDelegate<Client> del)
        {
            GetClient().RemoveMessageDelegate(messageId, del);
        }

        public void RemoveServerMessageDelegates(Object target)
        {
            GetClient().RemoveDelegates(target);
        }

        public void SetServerListener(IServerListener listener)
        {
            serverListener = listener;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

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
