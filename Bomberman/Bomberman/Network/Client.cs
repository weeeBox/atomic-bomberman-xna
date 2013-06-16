using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;
using BomberEngine.Debugging;
using BomberEngine.Core.IO;

namespace Bomberman.Network
{
    public interface ClientListener
    {
        void OnMessageReceived(Client client, Connection connection, NetworkMessage message);
        void OnConnectedToServer(Client client, Connection serverConnection);
        void OnDisconnectedFromServer(Client client);
    }

    public class Client : Peer
    {
        private enum State
        {
            Created,
            Connecting,
            Connected
        }

        public ClientListener listener;

        private IPEndPoint endPoint;
        private Connection serverConnection;

        private State state;

        public Client(String name, IPEndPoint endPoint)
            : base(name, endPoint.Port)
        {
            this.endPoint = endPoint;
            state = State.Created;
        }

        public override void Start()
        {
            if (peer != null)
            {
                throw new InvalidOperationException("Client already running");
            }

            Debug.Assert(state == State.Created);

            NetPeerConfiguration config = new NetPeerConfiguration(name);

            peer = new NetClient(config);
            peer.Start();

            state = State.Connecting;

            NetOutgoingMessage hailMessage = peer.CreateMessage();
            hailMessage.Write(CVars.name.value);
            peer.Connect(endPoint);
        }

        public override void Stop()
        {
            if (peer != null)
            {
                peer.Shutdown("disconnect");
                peer = null;
                serverConnection = null;
            }
        }

        protected override void OnPeerConnected(Connection connection)
        {
            Log.i("Connected to the server: " + connection.GetRemoteEndPoint());
            Debug.Assert(serverConnection == null);
            serverConnection = connection;

            listener.OnConnectedToServer(this, serverConnection);
        }

        protected override void OnPeerDisconnected(Connection connection)
        {
            Log.i("Disconnected from the server: " + connection.GetRemoteEndPoint());
            Debug.Assert(serverConnection == connection);

            serverConnection = null;
            listener.OnDisconnectedFromServer(this);
        }

        protected override void OnMessageReceive(Connection connection, NetworkMessage message)
        {
            listener.OnMessageReceived(this, connection, message);
        }
    }
}
