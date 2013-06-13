using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using Lidgren.Network;
using System.Net;
using BomberEngine.Core.IO;
using BomberEngine.Debugging;

namespace Bomberman.Network
{
    public abstract class NetworkPeer : IUpdatable
    {
        public enum NetworkMessageID
        {
            FieldStateRequest,
            FieldStateResponse,
        }

        protected NetPeer peer;

        protected String name;
        protected int port;

        private BitReadBuffer readBuffer;
        private BitWriteBuffer writeBuffer;

        protected NetworkPeer(String name, int port)
        {
            this.name = name;
            this.port = port;

            readBuffer = new BitReadBuffer();
            writeBuffer = new BitWriteBuffer();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        public abstract void Start();
        public abstract void Stop();

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Updatable

        public void Update(float delta)
        {
            NetIncomingMessage msg;
            while ((msg = peer.ReadMessage()) != null)
            {
                HandleMessage(peer, msg);
            }
        }

        protected virtual bool HandleMessage(NetPeer peer, NetIncomingMessage msg)
        {
            switch (msg.MessageType)
            {
                case NetIncomingMessageType.StatusChanged:
                {
                    NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                    if (status == NetConnectionStatus.Connected)
                    {   
                        OnPeerConnected(msg.SenderConnection);
                        return true;
                    }

                    if (status == NetConnectionStatus.Disconnected)
                    {
                        OnPeerDisconnected(msg.SenderConnection);
                        return true;
                    }

                    return false;
                }

                case NetIncomingMessageType.Data:
                {
                    ReadMessage(msg);
                    break;
                }
            }

            return false;
        }

        protected virtual void OnPeerConnected(NetConnection connection)
        {
        }

        protected virtual void OnPeerDisconnected(NetConnection connection)
        {
        }

        protected virtual void OnMessageReceive(NetConnection connection, NetworkMessageID message, BitReadBuffer buffer)
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Messages

        private void ReadMessage(NetIncomingMessage msg)
        {
            readBuffer.Init(msg.Data, msg.LengthBits);
            NetworkMessageID id = (NetworkMessageID)readBuffer.ReadByte();
            OnMessageReceive(msg.SenderConnection, id, readBuffer);
            readBuffer.Reset();
        }

        protected void SendMessage(NetConnection connection, NetworkMessageID message)
        {
            BitWriteBuffer buffer = GetWriteBuffer(message);
            SendBuffer(connection, buffer);
        }

        protected void SendBuffer(NetConnection connection, BitWriteBuffer buffer)
        {
            byte[] data = buffer.Data;
            int length = buffer.LengthBytes;

            NetOutgoingMessage message = peer.CreateMessage(length);
            message.Write(data, 0, length);

            peer.SendMessage(message, connection, NetDeliveryMethod.Unreliable);
        }

        protected BitWriteBuffer GetWriteBuffer(NetworkMessageID message)
        {
            writeBuffer.Reset();

            byte id = (byte)message;
            writeBuffer.Write(id);

            return writeBuffer;
        }
            
        #endregion
    }
}
