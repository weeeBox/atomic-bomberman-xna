using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using Lidgren.Network;
using System.Net;
using BomberEngine.Core.IO;
using BomberEngine.Debugging;

namespace Bomberman.Networking
{
    public enum NetworkMessageId
    {   
        FieldState,
        PlayerActions,
        Count,
    }

    public abstract class Peer : IUpdatable
    {
        protected String name;
        protected int port;

        protected NetPeer peer;

        protected Peer(String name, int port)
        {
            this.name = name;
            this.port = port;
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
            while (peer != null && (msg = peer.ReadMessage()) != null)
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

        protected virtual void OnMessageReceive(NetworkMessageId messageId, NetIncomingMessage message)
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Messages

        private void ReadMessage(NetIncomingMessage msg)
        {
            NetworkMessageId message = ReadMessageId(msg);
            OnMessageReceive(message, msg);
        }

        public void SendMessage(NetOutgoingMessage message, NetConnection recipient, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
        {
            peer.SendMessage(message, recipient, method);
        }

        public void SendMessage(NetworkMessageId messageId, NetConnection recipient, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
        {
            NetOutgoingMessage message = CreateMessage(messageId);
            peer.SendMessage(message, recipient, method);
        }

        public abstract void SendMessage(NetOutgoingMessage message, NetDeliveryMethod method = NetDeliveryMethod.Unreliable);
        public abstract void SendMessage(NetworkMessageId messageId, NetDeliveryMethod method = NetDeliveryMethod.Unreliable);
        
        public NetOutgoingMessage CreateMessage()
        {
            return peer.CreateMessage();
        }

        public NetOutgoingMessage CreateMessage(NetworkMessageId messageId)
        {
            NetOutgoingMessage message = CreateMessage();
            WriteMessageId(messageId, message);
            return message;
        }

        public NetOutgoingMessage CreateMessage(int initialCapacity)
        {
            return peer.CreateMessage(initialCapacity);
        }

        public NetOutgoingMessage CreateMessage(NetworkMessageId messageId, int initialCapacity)
        {
            NetOutgoingMessage message = CreateMessage(initialCapacity);
            WriteMessageId(messageId, message);
            return message;
        }

        private void WriteMessageId(NetworkMessageId messageId, NetOutgoingMessage message)
        {
            byte id = (byte)messageId;
            message.Write(id, 4);
        }

        private NetworkMessageId ReadMessageId(NetIncomingMessage message)
        {
            byte id = message.ReadByte(4);
            return (NetworkMessageId)id;
        }
            
        #endregion
    }
}
