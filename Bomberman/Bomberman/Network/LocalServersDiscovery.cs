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

    public class LocalServersDiscovery : Peer
    {   
        private LocalServersDiscoveryDelegate searchDelegate;

        private int serverIndex;

        public LocalServersDiscovery(LocalServersDiscoveryDelegate searchDelegate, String name, int port)
            : base(name, port)
        {
            this.searchDelegate = searchDelegate;
        }

        public override void Start()
        {
            if (peer != null)
            {
                throw new InvalidOperationException("Client already running");
            }

            NetPeerConfiguration config = new NetPeerConfiguration(name);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

            peer = new NetClient(config);
            peer.Start();

            peer.DiscoverLocalPeers(port);
        }

        public override void Stop()
        {
            if (peer != null)
            {
                peer.Shutdown("disconnect");
                peer = null;
            }
        }

        protected override bool HandleMessage(NetPeer peer, NetIncomingMessage msg)
        {
            switch (msg.MessageType)
            {
                case NetIncomingMessageType.DiscoveryResponse:
                {
                    String name = "Server " + (++serverIndex);
                    IPEndPoint endPoint = msg.SenderEndPoint;

                    ServerInfo info = new ServerInfo(name, endPoint);
                    searchDelegate(info);

                    return true;
                }
            }

            return false;
        }
    }
}
