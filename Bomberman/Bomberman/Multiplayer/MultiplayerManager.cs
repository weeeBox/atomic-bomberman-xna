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
    public class MultiplayerManager : IUpdatable, ClientListener, ServerListener
    {
        private Peer networkPeer;
        private LocalServersDiscovery serverDiscovery;

        private ClientListener clientListener;
        private ServerListener serverListener;

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

        public void StartLocalServerDiscovery(ILocalServersDiscoveryResponseListener listener, String name, int port)
        {
            Debug.Assert(serverDiscovery != null);
            
            serverDiscovery = new LocalServersDiscovery(listener, name, port);
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

        public void StartListeningForServerDiscovery(ILocalServersDiscoveryRequestListener listener)
        {
            GetServer().StartListeningDiscoveryRequests(listener);
        }

        public void StopListeningForServerDiscovery()
        {
            GetServer().StopListeningDiscoveryRequests();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Net peer

        public void CreateServer(String appIdentifier, int port)
        {
            Debug.Assert(networkPeer == null);
            Server server = new Server(appIdentifier, port);
            server.listener = this;
            networkPeer = server;

            Log.d("Created network server");
        }

        public void CreateClient(String appIdetifier, IPEndPoint remoteEndPoint, int port)
        {
            Debug.Assert(networkPeer == null);
            Client client = new Client(appIdetifier, remoteEndPoint);
            client.listener = this;
            networkPeer = client;

            Log.d("Created network client");
        }

        public void Start()
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

            clientListener = null;
            serverListener = null;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Client listener

        public void OnMessageReceived(Client client, NetworkMessageId messageId, NetIncomingMessage message)
        {
            if (clientListener != null)
            {
                clientListener.OnMessageReceived(client, messageId, message);
            }
        }

        public void OnConnectedToServer(Client client, NetConnection serverConnection)
        {
            if (clientListener != null)
            {
                clientListener.OnConnectedToServer(client, serverConnection);
            }
        }

        public void OnDisconnectedFromServer(Client client)
        {
            if (clientListener != null)
            {
                clientListener.OnDisconnectedFromServer(client);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Server listener

        public void OnMessageReceived(Server server, NetworkMessageId messageId, NetIncomingMessage message)
        {
            if (serverListener != null)
            {
                serverListener.OnMessageReceived(server, messageId, message);
            }
        }

        public void OnClientConnected(Server server, string name, NetConnection connection)
        {
            if (serverListener != null)
            {
                serverListener.OnClientConnected(server, name, connection);
            }
        }

        public void OnClientDisconnected(Server server, NetConnection connection)
        {
            if (serverListener != null)
            {
                serverListener.OnClientDisconnected(server, connection);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public void SetServerListener(ServerListener listener)
        {
            serverListener = listener;
        }

        public void SetClientListener(ClientListener listener)
        {
            clientListener = listener;
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
