using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;
using BomberEngine.Debugging;
using Bomberman.Network.Requests;

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

        private NetClient client;
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
            if (client != null)
            {
                throw new InvalidOperationException("Client already running");
            }

            Debug.Assert(state == State.Created);

            NetPeerConfiguration config = new NetPeerConfiguration(name);

            client = new NetClient(config);
            client.Start();

            state = State.Connecting;
            client.Connect(endPoint);
        }

        public override void Stop()
        {
            if (client != null)
            {
                client.Shutdown("disconnect");
                client = null;
            }
        }

        public override void Update(float delta)
        {
            NetIncomingMessage msg;
            while ((msg = client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                    {
                        NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                        if (status == NetConnectionStatus.Connected)
                        {
                            Debug.Assert(state == State.Connecting);
                            state = State.Connected;
                        }
                        break;
                    }
                    case NetIncomingMessageType.Data:
                        break;
                }
            }
        }
    }
}
