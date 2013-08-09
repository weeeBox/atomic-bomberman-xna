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
using Bomberman.Multiplayer;

namespace Bomberman.Networking
{
    public class LocalServersDiscovery : Peer
    {   
        public LocalServersDiscovery(String name, int port)
            : base(name, port)
        {
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

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

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Inheritance

        protected override bool HandleMessage(NetPeer peer, NetIncomingMessage msg)
        {
            switch (msg.MessageType)
            {
                case NetIncomingMessageType.DiscoveryResponse:
                {   
                    PostNotificationImmediately(NetworkNotifications.LocalServerDiscovered, msg);
                    return true;
                }
            }

            return false;
        }

        public override void SendMessage(NetOutgoingMessage message, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced)
        {
            throw new InvalidOperationException();
        }

        public override void SendMessage(NetworkMessageId messageId, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }
}
