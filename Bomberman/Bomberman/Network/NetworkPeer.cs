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

        private BitBufferReader reader;
        private BitBufferWriter writer;

        protected NetworkPeer(String name, int port)
        {
            this.name = name;
            this.port = port;

            reader = new BitBufferReader();
            writer = new BitBufferWriter();

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
                        OnPeerConnected(msg.SenderEndPoint);
                        return true;
                    }

                    if (status == NetConnectionStatus.Disconnected)
                    {
                        OnPeerDisconnected(msg.SenderEndPoint);
                        return true;
                    }

                    return false;
                }

                case NetIncomingMessageType.Data:
                {
                    reader.Init(msg.Data, msg.LengthBits);
                    NetworkMessage message = ReadMessage(reader);
                    reader.Reset();
                    OnMessageReceive(message);
                    break;
                }
            }

            return false;
        }

        protected virtual void OnPeerConnected(IPEndPoint endPoint)
        {
        }

        protected virtual void OnPeerDisconnected(IPEndPoint endPoint)
        {
        }

        protected virtual void OnMessageReceive(NetworkMessage message)
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Messages

        protected void WriteMessage(NetworkMessage message)
        {
            WriteMessage(writer, message);
        }

        protected void WriteMessage(NetworkMessageID messageId)
        {
            WriteMessage(writer, messageId);
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

        private NetworkMessage ReadMessage(BitBufferReader reader)
        {
            NetworkMessageID id = (NetworkMessageID)reader.ReadByte();

            NetworkMessage message = FindMessageObject(id);
            Debug.Assert(message != null);

            message.Read(reader);
            return message;
        }

        private void WriteMessage(BitBufferWriter writer, NetworkMessageID messageId)
        {
            NetworkMessage message = FindMessageObject(messageId);
            Debug.Assert(message != null);

            WriteMessage(writer, message);
        }

        private void WriteMessage(BitBufferWriter writer, NetworkMessage message)
        {
            writer.Reset();

            byte id = (byte)message.id;

            writer.Write(id);
            message.Write(writer);
        }

        private void Write(BitBufferWriter writer)
        {
            
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
