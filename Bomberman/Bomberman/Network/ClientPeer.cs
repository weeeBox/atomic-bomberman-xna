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
    public class ClientPeer : NetworkPeer
    {
        private enum State
        {
            Created,
            Connecting,
            Connected
        }

        private IPEndPoint endPoint;
        private State state;

        public ClientPeer(String name, IPEndPoint endPoint)
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
            }
        }

        protected override void OnPeerConnected(IPEndPoint endPoint)
        {
            Log.i("Connected to the server: " + endPoint);
        }

        protected override void OnPeerDisconnected(IPEndPoint endPoint)
        {
            Log.i("Disconnected from the server: " + endPoint);
        }
    }
}
