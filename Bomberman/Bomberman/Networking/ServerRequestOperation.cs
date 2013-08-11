using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Operations;
using Lidgren.Network;
using BomberEngine.Game;
using BomberEngine.Debugging;
using BomberEngine.Core;

namespace Bomberman.Networking
{
    public delegate void ServerRequestFinishDelegate(ServerRequestOperation op);
    public delegate void ServerRequestPeerDelegate(ServerRequestOperation op, NetIncomingMessage message);

    public class ServerRequestOperation : BaseOperation
    {
        private Server m_server;
        private NetworkRequestId m_requestId;
        private NetOutgoingMessage m_payloadMessage;

        private ServerRequestPeerDelegate m_peerDelegate;
        private ServerRequestFinishDelegate m_finishDelegate;

        private List<NetConnection> m_receivedConnections;

        public ServerRequestOperation(Server server, NetworkRequestId requestId, ServerRequestFinishDelegate finishDelegate, NetOutgoingMessage payloadMessage = null)
        {
            m_server = server;
            m_requestId = requestId;
            m_finishDelegate = finishDelegate;
            m_payloadMessage = payloadMessage;
            m_receivedConnections = new List<NetConnection>();
        }

        protected override void OnStart()
        {
            Application.ScheduleTimerOnce(DoWorkCallback);
        }

        protected override void DoWork()
        {
            m_server.AddMessageDelegate(NetworkMessageId.Response, ResponseReceived);

            NetOutgoingMessage msg = m_server.CreateMessage(NetworkMessageId.Request);
            msg.Write((byte)m_requestId);
            if (m_payloadMessage != null)
            {
                msg.Write(m_payloadMessage);
                m_server.RecycleMessage(m_payloadMessage);
            }

            m_server.SendMessage(msg, NetDeliveryMethod.ReliableUnordered);
        }

        protected override void OnFinish()
        {
            m_server.RemoveMessageDelegates(this);
            Application.CancelAllTimers(this);
        }

        private void ResponseReceived(Peer peer, NetworkMessageId messageId, NetIncomingMessage message)
        {
            Debug.Assert(messageId == NetworkMessageId.Response);
            NetworkRequestId id = (NetworkRequestId)message.ReadByte();

            if (id == m_requestId)
            {
                NetConnection connection = message.SenderConnection;
                if (!m_receivedConnections.Contains(connection))
                {
                    m_receivedConnections.Add(connection);
                    if (m_peerDelegate != null)
                    {
                        m_peerDelegate(this, message);
                    }

                    Server server = peer as Server;
                    Debug.AssertNotNull(server);

                    if (m_receivedConnections.Count == server.GetConnectionsCount())
                    {
                        m_receivedConnections.Clear();
                        m_finishDelegate(this);
                        Finish();
                    }
                }
            }
        }

        private void DoWorkCallback(Timer timer)
        {
            DoWork();
        }

        public Server server
        {
            get { return m_server; }
        }

        public NetworkRequestId requestId
        {
            get { return m_requestId; }
        }
    }
}
