using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using BomberEngine.Debugging;
using System.Net;

namespace Bomberman.Network
{
    public class ServerPeer : NetworkPeer
    {
        private NetServer server;
        private IDictionary<IPEndPoint, NetworkPlayer> players;
        
        public ServerPeer(String name, int port)
            : base(name, port)
        {
            players = new Dictionary<IPEndPoint, NetworkPlayer>();
        }

        public override void Start()
        {
            if (server != null)
            {
                throw new InvalidOperationException("Server already started");
            }

            NetPeerConfiguration config = new NetPeerConfiguration(name);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = port;

            server = new NetServer(config);
            server.Start();
        }

        public override void Stop()
        {
            if (server != null)
            {
                server.Shutdown("shutdown");
                server = null;
            }
        }

        public override void Update(float delta)
        {
            NetIncomingMessage msg;
            while ((msg = server.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryRequest:
                    {   
                        server.SendDiscoveryResponse(null, msg.SenderEndPoint);
                        break;
                    }

                    case NetIncomingMessageType.StatusChanged:
                    {
                        NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                        if (status == NetConnectionStatus.Connected)
                        {
                            AddClient(msg.SenderEndPoint);
                        }
                        else if (status == NetConnectionStatus.Disconnected)
                        {
                            RemoveClient(msg.SenderEndPoint);
                        }
                        break;
                    }

                    case NetIncomingMessageType.Data:
                    {
                        break;
                    }
                }
            }
        }

        private void AddClient(IPEndPoint endPoint)
        {
            Debug.Assert(!players.ContainsKey(endPoint));
            players.Add(endPoint, new NetworkPlayer("Player", endPoint));

            Log.i("Client connected: " + endPoint);
        }

        private void RemoveClient(IPEndPoint endPoint)
        {
            Debug.Assert(players.ContainsKey(endPoint));
            players.Remove(endPoint);

            Log.i("Client disconnected: " + endPoint);
        }

        private NetworkPlayer FindClient(IPEndPoint endPoint)
        {
            NetworkPlayer player;
            if (players.TryGetValue(endPoint, out player))
            {
                return player;
            }
            return null;
        }
    }
}
