﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using Lidgren.Network;
using System.Net;
using BomberEngine.Core.IO;
using BomberEngine.Debugging;
using BomberEngine.Game;

namespace Bomberman.Networking
{
    public enum NetworkMessageId
    {   
        Request,
        Response,

        FieldState,
        RoundEnded,
        GameEnded,
        ClientPacket,
        ServerPacket,
        Count,
    }

    public abstract class Peer : IUpdatable
    {
        protected String m_name;
        protected int m_port;

        protected NetPeer m_peer;

        protected Peer(String name, int port)
        {
            m_name = name;
            m_port = port;
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
            while (m_peer != null && (msg = m_peer.ReadMessage()) != null)
            {
                HandleMessage(m_peer, msg);
            }
        }

        protected virtual bool HandleMessage(NetPeer peer, NetIncomingMessage msg)
        {
            Log.d(msg.MessageType != NetIncomingMessageType.Data, "Message received: " + msg.MessageType);

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
            m_peer.SendMessage(message, recipient, method);
        }

        public void SendMessage(NetworkMessageId messageId, NetConnection recipient, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
        {
            NetOutgoingMessage message = CreateMessage(messageId);
            m_peer.SendMessage(message, recipient, method);
        }

        public abstract void SendMessage(NetOutgoingMessage message, NetDeliveryMethod method = NetDeliveryMethod.Unreliable);
        public abstract void SendMessage(NetworkMessageId messageId, NetDeliveryMethod method = NetDeliveryMethod.Unreliable);
        
        public NetOutgoingMessage CreateMessage()
        {
            return m_peer.CreateMessage();
        }

        public NetOutgoingMessage CreateMessage(NetworkMessageId messageId)
        {
            NetOutgoingMessage message = CreateMessage();
            WriteMessageId(messageId, message);
            return message;
        }

        public NetOutgoingMessage CreateMessage(int initialCapacity)
        {
            return m_peer.CreateMessage(initialCapacity);
        }

        public NetOutgoingMessage CreateMessage(NetworkMessageId messageId, int initialCapacity)
        {
            NetOutgoingMessage message = CreateMessage(initialCapacity);
            WriteMessageId(messageId, message);
            return message;
        }

        public void RecycleMessage(NetOutgoingMessage msg)
        {
            m_peer.Recycle(msg);
        }

        public void RecycleMessage(NetIncomingMessage msg)
        {
            m_peer.Recycle(msg);
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

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        protected void PostNotification(String name, Object data2 = null, Object data3 = null, Object data4 = null)
        {
            Application.NotificationCenter().Post(name, this, data2, data3, data4);
        }

        protected void PostNotificationImmediately(String name, Object data2 = null, Object data3 = null, Object data4 = null)
        {
            Application.NotificationCenter().PostImmediately(name, this, data2, data3, data4);
        }

        #endregion

        public virtual NetConnection RemoteConnection
        {
            get { throw new NotImplementedException("Should be implemented in a subclass");  }
        }
    }
}
