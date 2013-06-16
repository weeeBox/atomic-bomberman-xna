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

namespace Bomberman.Networking
{
    public interface ServerListener
    {
        void OnMessageReceived(Server server, NetworkMessageId messageId, NetIncomingMessage message);
        void OnClientConnected(Server server, NetConnection connection);
        void OnClientDisconnected(Server server, NetConnection connection);
    }

    public class Server : Peer
    {
        public ServerListener listener;
        public ServerInfo serverInfo;

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

            String hostName = CVars.sv_hostname.value;
            serverInfo = new ServerInfo(hostName);

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
                    NetOutgoingMessage message = peer.CreateMessage();
                    LocalServersDiscovery.WriteDiscoveryResponse(message, serverInfo);
                    peer.SendDiscoveryResponse(message, msg.SenderEndPoint);
                    return true;
                }
            }

            return false;
        }

        protected override void OnPeerConnected(NetConnection connection)
        {
            Log.i("Client connected: " + connection);
            listener.OnClientConnected(this, connection);
        }

        protected override void OnPeerDisconnected(NetConnection connection)
        {   
            Log.i("Client disconnected: " + connection);
            listener.OnClientDisconnected(this, connection);
        }

        protected override void OnMessageReceive(NetworkMessageId messageId, NetIncomingMessage message)
        {
            listener.OnMessageReceived(this, messageId, message);
        }
    }
}
