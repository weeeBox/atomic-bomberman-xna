using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;
using BomberEngine.Core.IO;

namespace Bomberman.Network
{
    public class Connection
    {
        internal String name;
        internal NetConnection connection;
        internal NetPeer peer;

        private BitWriteBuffer writeBuffer;
        private BitReadBuffer readBuffer;

        public Connection(NetPeer peer, NetConnection connection)
        {
            this.peer = peer;
            this.connection = connection;

            writeBuffer = new BitWriteBuffer();
            readBuffer = new BitReadBuffer();
        }

        public void SendMessage(NetworkMessage message)
        {
            byte id = (byte)message;
            NetOutgoingMessage msg = peer.CreateMessage(1);
            msg.Write(id);
            peer.SendMessage(msg, connection, NetDeliveryMethod.Unreliable);
        }

        public void SendMessage(NetworkMessage message, BitWriteBuffer buffer)
        {
            byte id = (byte)message;

            byte[] data = buffer.Data;
            int dataLength = buffer.LengthBytes;
            NetOutgoingMessage msg = peer.CreateMessage(1 + dataLength);
            msg.Write(id);
            msg.Write(data, 0, dataLength);
            peer.SendMessage(msg, connection, NetDeliveryMethod.Unreliable);
        }

        public BitWriteBuffer WriteBuffer
        {
            get
            {
                writeBuffer.Reset();
                return writeBuffer;
            }
        }

        public BitReadBuffer ReadBuffer
        {
            get
            {
                return readBuffer;
            }
        }

        internal BitReadBuffer CreateReadBuffer(byte[] data, int bitLength)
        {
            readBuffer.Init(data, bitLength);
            return readBuffer;
        }

        public IPEndPoint GetRemoteEndPoint()
        {
            return connection.RemoteEndPoint;
        }

        public String GetName()
        {
            return name;
        }
    }
}
