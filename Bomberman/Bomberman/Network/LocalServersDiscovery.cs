using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;

namespace Bomberman.Network
{
    public delegate void LocalServersDiscoveryDelegate(ServerInfo info);

    public class ServerInfo
    {
        public String name;
        public IPEndPoint endPoint;

        public ServerInfo(String name, IPEndPoint endPoint)
        {
            this.name = name;
            this.endPoint = endPoint;
        }
    }

    public class LocalServersDiscovery : NetworkPeer
    {
        private NetClient client;
        private LocalServersDiscoveryDelegate searchDelegate;

        private int serverIndex;

        public LocalServersDiscovery(LocalServersDiscoveryDelegate searchDelegate, String name, int port)
            : base(name, port)
        {
            this.searchDelegate = searchDelegate;
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
                    {
                        String name = "Server " + (++serverIndex);
                        IPEndPoint endPoint = msg.SenderEndPoint;

                        ServerInfo info = new ServerInfo(name, endPoint);
                        searchDelegate(info);
                        break;
                    }
                }
            }
        }
    }
}
