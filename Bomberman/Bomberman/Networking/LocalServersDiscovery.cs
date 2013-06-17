using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;
using Bomberman.Game.Elements.Fields;
using BomberEngine.Core.IO;
using Bomberman.Content;
using BombermanCommon.Resources.Scheme;

namespace Bomberman.Networking
{
    public interface LocalServersDiscoveryListener
    {
        void OnServerDiscoveryResponse(LocalServersDiscovery serverDiscovery, NetIncomingMessage msg);
    }

    public class LocalServersDiscovery : Peer
    {   
        private LocalServersDiscoveryListener listener;

        public LocalServersDiscovery(LocalServersDiscoveryListener listener, String name, int port)
            : base(name, port)
        {
            this.listener = listener;
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
                    listener.OnServerDiscoveryResponse(this, msg);
                    return true;
                }
            }

            return false;
        }
    }
}
