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
        Discovery,
        FieldState,
        Count,
    }

    public abstract class Peer : IUpdatable
    {
        protected String name;
        protected int port;

        protected NetPeer peer;
        protected List<NetConnection> connections;

        protected Peer(String name, int port)
        {
            this.name = name;
            this.port = port;

            connections = new List<NetConnection>();
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
            if (msg.SenderConnection != null && msg.SenderConnection.RemoteHailMessage != null)
            {
                String serverName = msg.SenderConnection.RemoteHailMessage.ReadString();
                Log.d("Server name: " + serverName);
            }

            switch (msg.MessageType)
            {
                case NetIncomingMessageType.StatusChanged:
                {
                    NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                    if (status == NetConnectionStatus.Connected)
                    {
                        NetConnection connection = msg.SenderConnection;
                        AddConnection(connection);
                        OnPeerConnected(connection);
                        return true;
                    }

                    if (status == NetConnectionStatus.Disconnected)
                    {
                        NetConnection connection = msg.SenderConnection;
                        RemoveConnection(connection);
                        OnPeerDisconnected(connection);
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

        #region Connections

        private void AddConnection(NetConnection c)
        {
            Debug.Assert(!connections.Contains(c));
            connections.Add(c);
        }

        private void RemoveConnection(NetConnection c)
        {
            bool removed = connections.Remove(c);
            Debug.Assert(removed);
        }

        public List<NetConnection> GetConnections()
        {
            return connections;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Messages

        private void ReadMessage(NetIncomingMessage msg)
        {
            NetworkMessageId message = ReadMessageId(msg);
            OnMessageReceive(message, msg);
        }

        public void SendMessage(NetOutgoingMessage message, NetConnection recipient)
        {
            peer.SendMessage(message, recipient, NetDeliveryMethod.Unreliable);
        }

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
