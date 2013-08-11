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
            if (m_peer != null)
            {
                throw new InvalidOperationException("Client already running");
            }

            NetPeerConfiguration config = new NetPeerConfiguration(m_name);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

            m_peer = new NetClient(config);
            m_peer.Start();

            m_peer.DiscoverLocalPeers(m_port);
        }

        public override void Stop()
        {
            if (m_peer != null)
            {
                m_peer.Shutdown("disconnect");
                m_peer = null;
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

        public override void SendMessage(NetOutgoingMessage message)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }
}
