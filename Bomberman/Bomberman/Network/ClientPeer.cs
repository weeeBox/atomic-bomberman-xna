using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;
using BomberEngine.Debugging;

namespace Bomberman.Network
{
    public class ClientPeer : NetworkPeer
    {
        private NetClient client;
        private IPEndPoint endPoint;

        public ClientPeer(String name, IPEndPoint endPoint)
            : base(name, endPoint.Port)
        {
            this.endPoint = endPoint;
        }

        public override void Start()
        {
            if (client != null)
            {
                throw new InvalidOperationException("Client already running");
            }

            NetPeerConfiguration config = new NetPeerConfiguration(name);

            client = new NetClient(config);
            client.Start();

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
                            Log.d("Connected to the server");
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
