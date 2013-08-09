using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Operations;
using BomberEngine.Core;
using Bomberman.Networking;
using Lidgren.Network;
using BomberEngine.Debugging;

namespace Bomberman.Multiplayer
{
    public enum NetworkRequestId
    {
        RoundStart,
        RoundEnd,
        GameEnd,
    }

    public delegate void NetworkRequestDelegate(NetworkRequest request);

    public class NetworkRequest : BaseOperation
    {
        private Peer m_peer;
        private NetworkRequestId m_requestId;
        private NetOutgoingMessage m_payloadMessage;
        private NetworkRequestDelegate m_delegate;
        private float m_timeout;

        public NetworkRequest(Peer peer, NetworkRequestId requestId, NetOutgoingMessage payloadMessage, float timeout = 0.0f)
        {
            m_peer = peer;
            m_requestId = requestId;
            m_payloadMessage = payloadMessage;
        }

        protected override void DoWork()
        {
            NetOutgoingMessage msg = m_peer.CreateMessage(NetworkMessageId.Request);
            msg.Write((byte)m_requestId);
            msg.Write(m_payloadMessage);

            m_peer.AddMessageDelegate(NetworkMessageId.Response, ResponseReceived);

            m_peer.RecycleMessage(m_payloadMessage);
            m_peer.SendMessage(msg, NetDeliveryMethod.ReliableUnordered);

            if (m_timeout > 0)
            {
                ScheduleTimerOnce(TimeoutCallback, m_timeout);
            }
        }

        protected override void OnFinish()
        {
            m_peer.RemoveMessageDelegates(this);
        }

        private void ResponseReceived(Peer peer, NetworkMessageId messageId, NetIncomingMessage message)
        {
            Debug.Assert(messageId == NetworkMessageId.Response);
            NetworkRequestId id = (NetworkRequestId) message.ReadByte();
            if (id == m_requestId)
            {
                if (m_delegate != null)
                {
                    m_delegate(this);
                }
                Finish();
            }
        }

        private void TimeoutCallback(Timer timer)
        {
            Cancel();
        }

        public Peer peer
        {
            get { return m_peer; }
        }

        public NetworkRequestId requestId
        {
            get { return m_requestId; }
        }

        public NetworkRequestDelegate requestDelegate
        {
            get { return m_delegate; }
            set { m_delegate = value; }
        }
    }
}
