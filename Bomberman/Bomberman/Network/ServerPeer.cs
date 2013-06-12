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
        private IDictionary<IPEndPoint, NetworkPlayer> players;
        
        public ServerPeer(String name, int port)
            : base(name, port)
        {
            players = new Dictionary<IPEndPoint, NetworkPlayer>();
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

        protected override void OnPeerConnected(IPEndPoint endPoint)
        {
            Debug.Assert(!players.ContainsKey(endPoint));
            players.Add(endPoint, new NetworkPlayer("Player", endPoint));

            Log.i("Client connected: " + endPoint);
        }

        protected override void OnPeerDisconnected(IPEndPoint endPoint)
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
