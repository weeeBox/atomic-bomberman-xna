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
    public enum NetworkMessage
    {
        FieldState,
    }

    public abstract class Peer : IUpdatable
    {
        protected String name;
        protected int port;

        protected NetPeer peer;
        protected List<Connection> connections;
        private IDictionary<NetConnection, Connection> connectionsLookup;

        protected Peer(String name, int port)
        {
            this.name = name;
            this.port = port;

            connections = new List<Connection>();
            connectionsLookup = new Dictionary<NetConnection, Connection>();
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
                        Connection connection = AddConnection(msg.SenderConnection);
                        OnPeerConnected(connection);
                        return true;
                    }

                    if (status == NetConnectionStatus.Disconnected)
                    {
                        Connection connection = RemoveConnection(msg.SenderConnection);
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

        protected virtual void OnPeerConnected(Connection connection)
        {
        }

        protected virtual void OnPeerDisconnected(Connection connection)
        {
        }

        protected virtual void OnMessageReceive(Connection connection, NetworkMessage message)
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Connections

        private Connection AddConnection(NetConnection c)
        {
            Debug.Assert(FindConnection(c) == null);

            Connection connection = new Connection(peer, c);
            connections.Add(connection);
            connectionsLookup.Add(c, connection);
            return connection;
        }

        private Connection RemoveConnection(NetConnection c)
        {
            Connection connection = FindConnection(c);
            Debug.Assert(connection != null);

            connections.Remove(connection);
            connectionsLookup.Remove(c);

            return connection;
        }

        private Connection FindConnection(NetConnection key)
        {
            Connection connection;
            if (connectionsLookup.TryGetValue(key, out connection))
            {
                return connection;
            }
            return null;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Messages

        private void ReadMessage(NetIncomingMessage msg)
        {
            Connection connection = FindConnection(msg.SenderConnection);
            Debug.Assert(connection != null);

            BitReadBuffer readBuffer = connection.CreateReadBuffer(msg.Data, msg.LengthBits);
            NetworkMessage message = (NetworkMessage)readBuffer.ReadByte();
            OnMessageReceive(connection, message);
            readBuffer.Reset();
        }
            
        #endregion
    }
}
