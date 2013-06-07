using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using BomberEngine.Debugging;

namespace Bomberman.Network
{
    public class GameServer : NetworkPeer
    {
        private NetServer server;
        
        public GameServer(String name, int port)
            : base(name, port)
        {   
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
                            Log.d("Client connected");
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
    }
}
