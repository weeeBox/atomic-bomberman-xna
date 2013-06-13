using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;
using BomberEngine.Debugging;
using Bomberman.Network.Requests;
using BomberEngine.Core.IO;

namespace Bomberman.Network
{
    public class NetworkClient : NetworkPeer
    {
        private enum State
        {
            Created,
            Connecting,
            Connected
        }

        private IPEndPoint endPoint;
        private NetConnection serverConnection;

        private State state;

        public NetworkClient(String name, IPEndPoint endPoint)
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

        protected override void OnPeerConnected(NetConnection connection)
        {
            Log.i("Connected to the server: " + connection.RemoteEndPoint);
            Debug.Assert(serverConnection == null);
            serverConnection = connection;

            WriteMessage(NetworkMessageID.FieldStateRequest);
        }

        protected override void OnPeerDisconnected(NetConnection connection)
        {
            Log.i("Disconnected from the server: " + connection.RemoteEndPoint);
            Debug.Assert(serverConnection == connection);

            serverConnection = null;
        }

        private void WriteMessage(NetworkMessageID messageId)
        {
            Debug.Assert(serverConnection != null);
            WriteMessage(serverConnection, messageId);
        }

        private void WriteMessage(NetworkMessage message)
        {
            Debug.Assert(serverConnection != null);
            WriteMessage(serverConnection, message);
        }
    }
}
