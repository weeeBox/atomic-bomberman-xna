using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;
using BomberEngine.Debugging;
using BomberEngine.Core.IO;

namespace Bomberman.Networking
{
    public interface ClientListener
    {
        void OnMessageReceived(Client client, NetworkMessageId messageId, NetIncomingMessage message);
        void OnConnectedToServer(Client client, NetConnection serverConnection);
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

        private IPEndPoint remoteEndPoint;
        private NetConnection serverConnection;

        private State state;

        public Client(String name, IPEndPoint remoteEndPoint)
            : base(name, remoteEndPoint.Port)
        {
            this.remoteEndPoint = remoteEndPoint;
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
            peer.Connect(remoteEndPoint, hailMessage);
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

        protected override void OnPeerConnected(NetConnection connection)
        {
            Log.i("Connected to the server: " + connection.RemoteEndPoint);
            Debug.Assert(serverConnection == null);
            serverConnection = connection;

            listener.OnConnectedToServer(this, serverConnection);
        }

        protected override void OnPeerDisconnected(NetConnection connection)
        {
            Log.i("Disconnected from the server: " + connection.RemoteEndPoint);
            Debug.Assert(serverConnection == connection);

            serverConnection = null;
            listener.OnDisconnectedFromServer(this);
        }

        protected override void OnMessageReceive(NetworkMessageId messageId, NetIncomingMessage message)
        {
            listener.OnMessageReceived(this, messageId, message);
        }
    }
}
