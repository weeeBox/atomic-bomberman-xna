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
    public delegate void LocalServersDiscoveryDelegate(ServerInfo info);

    public class ServerInfo
    {
        public String name;
        public IPEndPoint endPoint;

        public Scheme scheme;

        public ServerInfo(String name)
        {
            this.name = name;
        }

        public ServerInfo(String name, IPEndPoint endPoint)
        {
            this.name = name;
            this.endPoint = endPoint;
        }
    }

    public class LocalServersDiscovery : Peer
    {   
        private LocalServersDiscoveryDelegate searchDelegate;

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
                    ServerInfo info = ReadDiscoveryResponse(msg);
                    searchDelegate(info);

                    return true;
                }
            }

            return false;
        }

        internal static ServerInfo ReadDiscoveryResponse(NetIncomingMessage message)
        {
            // name
            String name = message.ReadString();

            // scheme
            Scheme scheme = new Scheme();

            // scheme: name
            scheme.name = message.ReadString();

            // scheme: field data
            int fieldWidth = message.ReadInt32();
            int fieldHeight = message.ReadInt32();
            FieldBlocks[] fieldDataArray = new FieldBlocks[fieldWidth * fieldHeight];
            for (int i = 0; i < fieldDataArray.Length; ++i)
            {
                fieldDataArray[i] = (FieldBlocks)message.ReadByte();
            }
            scheme.fieldData = new FieldData(fieldWidth, fieldHeight, fieldDataArray);

            // scheme: player locations
            int locationsCount = message.ReadByte();
            PlayerLocationInfo[] playerLocations = new PlayerLocationInfo[locationsCount];
            for (int i = 0; i < locationsCount; ++i)
            {
                int x = message.ReadByte();
                int y = message.ReadByte();
                int team = message.ReadByte();

                playerLocations[i] = new PlayerLocationInfo(i, x, y, team);
            }
            scheme.playerLocations = playerLocations;

            ServerInfo info = new ServerInfo(name, message.SenderEndPoint);
            info.scheme = scheme;
            return info;
        }

        internal static void WriteDiscoveryResponse(NetOutgoingMessage message, ServerInfo info)
        {
            // name
            message.Write(info.name);

            // scheme
            Scheme scheme = info.scheme;

            // scheme: name
            message.Write(scheme.name);

            // scheme: field data
            FieldData fieldData = scheme.fieldData;
            message.Write(fieldData.GetWidth());
            message.Write(fieldData.GetHeight());

            FieldBlocks[] blocks = fieldData.GetDataArray();
            for (int i = 0; i < blocks.Length; ++i)
            {
                byte block = (byte)blocks[i];
                message.Write(block);
            }

            // scheme: player locations
            PlayerLocationInfo[] playerLocations = scheme.GetPlayerLocations();
            message.Write((byte)playerLocations.Length);
            for (int i = 0; i < playerLocations.Length; ++i)
            {
                message.Write((byte)playerLocations[i].x);
                message.Write((byte)playerLocations[i].y);
                message.Write((byte)playerLocations[i].team);
            }
        }
    }
}
