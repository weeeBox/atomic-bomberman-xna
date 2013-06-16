using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using BomberEngine.Debugging;
using System.Net;
using Bomberman.Game.Elements.Fields;
using BomberEngine.Core.IO;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements;

namespace Bomberman.Network
{
    public interface ServerListener
    {
        void OnMessageReceived(Server server, Connection connection, NetworkMessage message);
        void OnClientConnected(Server server, Connection connection);
        void OnClientDisconnected(Server server, Connection connection);
        void WriteDiscoveryResponse(BitWriteBuffer buffer);
    }

    public class Server : Peer
    {
        public ServerListener listener;

        public Server(String name, int port)
            : base(name, port)
        {   
        }

        public override void Start()
        {
            if (peer != null)
            {
                throw new InvalidOperationException("Server already started");
            }

            NetPeerConfiguration config = new NetPeerConfiguration(name);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = port;

            peer = new NetServer(config);
            peer.Start();
        }

        public override void Stop()
        {
            if (peer != null)
            {
                peer.Shutdown("shutdown");
                peer = null;
            }
        }

        protected override bool HandleMessage(NetPeer peer, NetIncomingMessage msg)
        {
            if (base.HandleMessage(peer, msg))
            {
                return true;
            }

            switch (msg.MessageType)
            {
                case NetIncomingMessageType.DiscoveryRequest:
                {
                    BitWriteBuffer buffer = new BitWriteBuffer();
                    listener.WriteDiscoveryResponse(buffer);
                    NetOutgoingMessage message = peer.CreateMessage(buffer.LengthBytes);
                    peer.SendDiscoveryResponse(message, msg.SenderEndPoint);
                    return true;
                }
            }

            return false;
        }

        protected override void OnPeerConnected(Connection connection)
        {
            Log.i("Client connected: " + connection);
            listener.OnClientConnected(this, connection);
        }

        protected override void OnPeerDisconnected(Connection connection)
        {   
            Log.i("Client disconnected: " + connection);
            listener.OnClientDisconnected(this, connection);
        }

        protected override void OnMessageReceive(Connection connection, NetworkMessage message)
        {
            listener.OnMessageReceived(this, connection, message);
        }
    }
}
