using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;
using Bomberman.Game.Elements.Fields;
using BomberEngine.Core.IO;

namespace Bomberman.Network
{
    public delegate void LocalServersDiscoveryDelegate(ServerInfo info);

    public class ServerInfo
    {
        public String name;
        public IPEndPoint endPoint;

        public int mapWidth;
        public int mapHeight;

        public FieldCellType[] cells;

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
                    BitReadBuffer buffer = new BitReadBuffer();
                    buffer.Init(msg.Data, msg.LengthBits);

                    int width = buffer.ReadInt32();
                    int height = buffer.ReadInt32();

                    FieldCellType[] cells = new FieldCellType[width * height];
                    for (int i = 0; i < cells.Length; ++i)
                    {
                        FieldCellType cell = (FieldCellType)buffer.ReadByte();
                        cells[i] = cell;
                    }

                    String name = "Server " + (++serverIndex);
                    IPEndPoint endPoint = msg.SenderEndPoint;

                    ServerInfo info = new ServerInfo(name, endPoint);
                    info.mapWidth = width;
                    info.mapHeight = height;
                    info.cells = cells;

                    searchDelegate(info);

                    return true;
                }
            }

            return false;
        }
    }
}
