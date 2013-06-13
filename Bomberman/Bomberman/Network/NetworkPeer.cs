using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using Bomberman.Network.Requests;
using Lidgren.Network;
using System.Net;
using BomberEngine.Core.IO;
using BomberEngine.Debugging;

namespace Bomberman.Network
{
    public abstract class NetworkPeer : IUpdatable
    {
        protected NetPeer peer;

        protected String name;
        protected int port;

        protected IDictionary<NetworkMessageID, NetworkMessage> messagePool;

        private BitReadBuffer readBuffer;
        private BitWriteBuffer writeBuffer;

        protected NetworkPeer(String name, int port)
        {
            this.name = name;
            this.port = port;

            readBuffer = new BitReadBuffer();
            writeBuffer = new BitWriteBuffer();

            messagePool = new Dictionary<NetworkMessageID, NetworkMessage>();
            RegisterMessages();
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
                    readBuffer.Init(msg.Data, msg.LengthBits);
                    NetworkMessage message = ReadMessage(readBuffer);
                    readBuffer.Reset();
                    OnMessageReceive(message);
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

        protected virtual void OnMessageReceive(NetworkMessage message)
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Messages

        protected void WriteMessage(NetConnection connection, NetworkMessage message)
        {
            WriteMessage(connection, writeBuffer, message);
        }

        protected void WriteMessage(NetConnection connection, NetworkMessageID messageId)
        {
            WriteMessage(connection, writeBuffer, messageId);
        }

        protected NetworkMessage FindMessageObject(NetworkMessageID messageId)
        {
            NetworkMessage message;
            if (messagePool.TryGetValue(messageId, out message))
            {
                return message;
            }

            return null;
        }

        private NetworkMessage ReadMessage(BitReadBuffer buffer)
        {
            NetworkMessageID id = (NetworkMessageID)buffer.ReadByte();

            NetworkMessage message = FindMessageObject(id);
            Debug.Assert(message != null);

            message.Read(buffer);
            return message;
        }

        private void WriteMessage(NetConnection connection, BitWriteBuffer buffer, NetworkMessageID messageId)
        {
            NetworkMessage message = FindMessageObject(messageId);
            Debug.Assert(message != null);

            WriteMessage(connection, buffer, message);
        }

        private void WriteMessage(NetConnection connection, BitWriteBuffer buffer, NetworkMessage message)
        {
            buffer.Reset();

            byte id = (byte)message.id;

            buffer.Write(id);
            message.Write(buffer);

            Write(connection, buffer);
        }

        private void Write(NetConnection connection, BitWriteBuffer buffer)
        {
            byte[] data = buffer.Data;
            int length = buffer.LengthBytes;

            NetOutgoingMessage message = peer.CreateMessage(length);
            message.Write(data, 0, length);

            peer.SendMessage(message, connection, NetDeliveryMethod.Unreliable);
        }

        private void RegisterMessages()
        {
            RegisterMessage(new MsgFieldStateRequest());
            RegisterMessage(new MsgFieldStateResponse());
        }

        private void RegisterMessage(NetworkMessage message)
        {   
            messagePool.Add(message.id, message);
        }
            
        #endregion
    }
}
