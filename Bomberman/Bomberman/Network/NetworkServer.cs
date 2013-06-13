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
    public class NetworkServer : NetworkPeer
    {   
        private IDictionary<NetConnection, NetworkPlayer> connections;
        
        public NetworkServer(String name, int port)
            : base(name, port)
        {
            connections = new Dictionary<NetConnection, NetworkPlayer>();
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
                    peer.SendDiscoveryResponse(null, msg.SenderEndPoint);
                    return true;
                }
            }

            return false;
        }

        protected override void OnPeerConnected(NetConnection connection)
        {
            Debug.Assert(!connections.ContainsKey(connection));
            connections.Add(connection, new NetworkPlayer("Player", connection));

            Log.i("Client connected: " + connection);
        }

        protected override void OnPeerDisconnected(NetConnection connection)
        {   
            Debug.Assert(connections.ContainsKey(connection));
            connections.Remove(connection);

            Log.i("Client disconnected: " + connection);
        }

        protected override void OnMessageReceive(NetConnection connection, NetworkMessage message, BitReadBuffer buffer)
        {
            NetworkPlayer player = FindClient(connection);
            Debug.Assert(player != null);

            Log.i("Message received: " + message + " from " + player.name);
            switch (message)
            {
                case NetworkMessage.FieldStateRequest:
                    SendFieldState(connection);
                    break;
            }
        }

        private NetworkPlayer FindClient(NetConnection connection)
        {
            NetworkPlayer player;
            if (connections.TryGetValue(connection, out player))
            {
                return player;
            }
            return null;
        }

        private const byte BLOCK_EMPTY = 0;
        private const byte BLOCK_SOLID = 1;
        private const byte BLOCK_BRICK = 2;

        private void SendFieldState(NetConnection connection)
        {
            BitWriteBuffer buffer = GetWriteBuffer(NetworkMessage.FieldStateResponse);
            GameNetwork.WriteFieldState(buffer);
            SendBuffer(connection, buffer);
        }
    }
}
