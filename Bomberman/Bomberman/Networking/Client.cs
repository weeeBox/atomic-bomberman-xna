using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;
using BomberEngine.Debugging;
using BomberEngine.Core.IO;
using Bomberman.Multiplayer;

namespace Bomberman.Networking
{
    using ClientReceivedMessageDelegate         = ReceivedMessageDelegate;
    using ClientReceivedMessageDelegateRegistry = ReceivedMessageDelegateRegistry;

    public class Client : Peer
    {
        private IPEndPoint m_remoteEndPoint;
        private NetConnection m_remoteConnection;

        private ClientReceivedMessageDelegateRegistry m_delegateRegistry;

        public Client(String name, IPEndPoint remoteEndPoint)
            : base(name, remoteEndPoint.Port)
        {
            this.m_remoteEndPoint = remoteEndPoint;
            m_delegateRegistry = new ClientReceivedMessageDelegateRegistry();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        public override void Start()
        {
            if (m_peer != null)
            {
                throw new InvalidOperationException("Client already running");
            }

            NetPeerConfiguration config = new NetPeerConfiguration(m_name);

            m_peer = new NetClient(config);
            m_peer.Start();

            NetOutgoingMessage hailMessage = m_peer.CreateMessage();
            hailMessage.Write(CVars.name.value);
            m_peer.Connect(m_remoteEndPoint, hailMessage);
        }

        public override void Stop()
        {
            if (m_peer != null)
            {
                m_peer.Shutdown("disconnect");
                m_peer = null;
                m_remoteConnection = null;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Inheritance

        protected override void OnPeerConnected(NetConnection connection)
        {
            Log.i("Connected to the server: " + connection.RemoteEndPoint);
            Debug.Assert(m_remoteConnection == null);
            m_remoteConnection = connection;

            PostNotification(NetworkNotifications.ConnectedToServer, connection);
        }

        protected override void OnPeerDisconnected(NetConnection connection)
        {
            Log.i("Disconnected from the server: " + connection.RemoteEndPoint);
            Debug.Assert(m_remoteConnection == connection);
            m_remoteConnection = null;

            PostNotification(NetworkNotifications.DisconnectedFromServer, connection);
        }

        protected override void OnMessageReceive(NetworkMessageId messageId, NetIncomingMessage message)
        {
            m_delegateRegistry.NotifyMessageReceived(this, messageId, message);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Message delegates

        public void AddMessageDelegate(NetworkMessageId messageId, ClientReceivedMessageDelegate del)
        {
            m_delegateRegistry.Add(messageId, del);
        }

        public void RemoveMessageDelegate(NetworkMessageId messageId, ClientReceivedMessageDelegate del)
        {
            m_delegateRegistry.Remove(messageId, del);
        }

        public void RemoveMessageDelegate(ClientReceivedMessageDelegate del)
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
            SendMessage(message, m_remoteConnection, method);
        }

        public override void SendMessage(NetworkMessageId messageId, NetDeliveryMethod method = NetDeliveryMethod.Unreliable)
        {   
            SendMessage(messageId, m_remoteConnection, method);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public override NetConnection RemoteConnection
        {
            get { return m_remoteConnection; } // TODO: use internal peer representation
        }

        #endregion
    }
}
