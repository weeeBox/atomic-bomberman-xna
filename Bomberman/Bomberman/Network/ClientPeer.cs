using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace Bomberman.Network
{
    public class ClientPeer : NetworkPeer
    {
        private NetClient client;

        public ClientPeer(String name, int port)
            : base(name, port)
        {
        }

        public override void Start()
        {
            if (client != null)
            {
                throw new InvalidOperationException("Client already running");
            }

            NetPeerConfiguration config = new NetPeerConfiguration(name);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

            client = new NetClient(config);
            client.Start();

            client.DiscoverLocalPeers(port);
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
                    case NetIncomingMessageType.DiscoveryResponse:
                        client.Connect(msg.SenderEndPoint);
                        break;
                    case NetIncomingMessageType.Data:
                        break;
                }
            }
        }
    }
}
