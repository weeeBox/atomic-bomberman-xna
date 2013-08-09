using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Operations;
using BomberEngine.Core;
using Bomberman.Networking;
using Lidgren.Network;

namespace Bomberman.Multiplayer
{
    public enum NetworkRequestId
    {
        RoundStart,
        RoundEnd,
        GameEnd,
    }

    public class NetworkRequest : BaseOperation
    {
        private Peer m_peer;
        private NetworkRequestId m_requestId;
        private NetOutgoingMessage m_payloadMessage;

        public NetworkRequest(Peer peer, NetworkRequestId requestId, NetOutgoingMessage payloadMessage)
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

            m_peer.RecycleMessage(m_payloadMessage);
            m_peer.SendMessage(msg, NetDeliveryMethod.ReliableUnordered);

            
        }

        protected override void OnFinish()
        {
            
        }

        private void ResponseReceived(Client client, NetworkMessageId messageId, NetIncomingMessage message)
        {

        }
    }
}
