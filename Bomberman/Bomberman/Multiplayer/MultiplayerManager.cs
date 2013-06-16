using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Networking;
using BomberEngine.Debugging;
using System.Net;
using BomberEngine.Core;

namespace Bomberman.Multiplayer
{
    public class MultiplayerManager : IUpdatable
    {
        private Peer networkPeer;

        public void Update(float delta)
        {
            if (networkPeer != null)
            {
                networkPeer.Update(delta);
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Net peer

        public void CreateServer(String appIdentifier, int port)
        {
            Debug.Assert(networkPeer == null);
            networkPeer = new Server(appIdentifier, port);

            Log.d("Created network server");
        }

        public void CreateClient(String appIdetifier, IPEndPoint remoteEndPoint, int port)
        {
            Debug.Assert(networkPeer == null);
            networkPeer = new Client(appIdetifier, remoteEndPoint);

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
            if (networkPeer != null)
            {
                networkPeer.Stop();
                networkPeer = null;

                Log.d("Stopped network peer");
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Delegates

        public void SetServerListener(ServerListener listener)
        {
            Debug.Assert(networkPeer != null);

            Server server = networkPeer as Server;
            Debug.Assert(server != null);

            server.listener = listener;
        }

        public void SetClientListener(ClientListener listener)
        {
            Debug.Assert(networkPeer != null);

            Client client = networkPeer as Client;
            Debug.Assert(client != null);

            client.listener = listener;
        }

        #endregion
    }
}
