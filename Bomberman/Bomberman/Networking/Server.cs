﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using BomberEngine.Debugging;
using System.Net;
using Bomberman.Game.Elements.Fields;
using BomberEngine.Core.IO;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements;
using Bomberman.Multiplayer;

namespace Bomberman.Networking
{
    using ServerReceivedMessageDelegate = ReceivedMessageDelegate;
    using ServerReceivedMessageDelegateRegistry = ReceivedMessageDelegateRegistry;

    public class Server : Peer
    {
        private int nextClientIndex;
        private List<NetConnection> connections;

        private bool m_respondsToDiscovery;
        private ServerReceivedMessageDelegateRegistry m_delegateRegistry;

        public Server(String name, int port)
            : base(name, port)
        {
            connections = new List<NetConnection>();
            m_delegateRegistry = new ServerReceivedMessageDelegateRegistry();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        public override void Start()
        {
            if (m_peer != null)
            {
                throw new InvalidOperationException("Server already started");
            }

            NetPeerConfiguration config = new NetPeerConfiguration(m_name);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = m_port;
            
            m_peer = new NetServer(config);
            m_peer.Start();
        }

        public override void Stop()
        {
            if (m_peer != null)
            {
                m_peer.Shutdown("shutdown");
                m_peer = null;
            }
        }

        public void StartListeningDiscoveryRequests()
        {
            m_respondsToDiscovery = true;
        }

        public void StopListeningDiscoveryRequests()
        {
            m_respondsToDiscovery = false;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Inheritance

        protected override bool HandleMessage(NetPeer peer, NetIncomingMessage msg)
        {
            if (base.HandleMessage(peer, msg))
            {
                return true;
            }

            switch (msg.MessageType)
            {
                case NetIncomingMessageType.DiscoveryRequest:
                {
                    if (m_respondsToDiscovery)
                    {
                        NetOutgoingMessage message = peer.CreateMessage();

                        PostNotificationImmediately(NetworkNotifications.LocalClientDiscovered, message);
                        peer.SendDiscoveryResponse(message, msg.SenderEndPoint);
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }

        protected override void OnPeerConnected(NetConnection connection)
        {
            Log.i("Client connected: " + connection);

            NetIncomingMessage hailMessage = connection.RemoteHailMessage;
            String name;
            if (hailMessage != null)
            {
                name = hailMessage.ReadString();
            }
            else
            {
                name = "Client-" + nextClientIndex++;
            }

            AddConnection(connection);
            PostNotification(NetworkNotifications.ClientConnected, connection, name);
        }

        protected override void OnPeerDisconnected(NetConnection connection)
        {   
            Log.i("Client disconnected: " + connection);
            RemoveConnection(connection);
            PostNotification(NetworkNotifications.ClientDisconnected, connection);
        }

        protected override void OnMessageReceive(NetworkMessageId messageId, NetIncomingMessage message)
        {
            m_delegateRegistry.NotifyMessageReceived(this, messageId, message);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Message delegates

        public void AddMessageDelegate(NetworkMessageId messageId, ServerReceivedMessageDelegate del)
        {
            m_delegateRegistry.Add(messageId, del);
        }

        public void RemoveMessageDelegate(NetworkMessageId messageId, ServerReceivedMessageDelegate del)
        {
            m_delegateRegistry.Remove(messageId, del);
        }

        public void RemoveMessageDelegate(ServerReceivedMessageDelegate del)
        {
            m_delegateRegistry.Remove(del);
        }

        public void RemoveMessageDelegates(Object target)
        {
            m_delegateRegistry.RemoveAll(target);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Messages

        public override void SendMessage(NetOutgoingMessage message, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
        {
            for (int i = 0; i < connections.Count; ++i)
            {
                SendMessage(message, connections[i], method);
            }
        }

        public override void SendMessage(NetworkMessageId messageId, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
        {
            for (int i = 0; i < connections.Count; ++i)
            {
                SendMessage(messageId, connections[i], method);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Connections

        public List<NetConnection> GetConnections()
        {
            return connections;
        }

        private void AddConnection(NetConnection connection)
        {
            Debug.Assert(!connections.Contains(connection));
            connections.Add(connection);
        }

        private void RemoveConnection(NetConnection connection)
        {
            Debug.Assert(connections.Contains(connection));
            connections.Remove(connection);
        }

        #endregion
    }
}
