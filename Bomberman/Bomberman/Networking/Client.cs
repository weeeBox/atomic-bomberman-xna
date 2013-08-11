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
    public class Client : Peer
    {
        private IPEndPoint m_remoteEndPoint;
        private NetConnection m_remoteConnection;

        public Client(String name, IPEndPoint remoteEndPoint)
            : base(name, remoteEndPoint.Port)
        {
            this.m_remoteEndPoint = remoteEndPoint;
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

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Messages

        public override void SendMessage(NetOutgoingMessage message)
        {
            SendMessage(message, m_remoteConnection);
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
