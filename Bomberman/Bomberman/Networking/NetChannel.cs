using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Players;
using Lidgren.Network;
using System.Net;
using BomberEngine;

namespace Bomberman.Networking
{
    public class NetChannel
    {
        private List<Player> m_players;     // players for this channel
        private Peer m_peer;                // network peer for this channel
        private NetConnection m_connection; // network connection for this channel

        public int outgoingSequence;        // the last sent packet sequence
        public int incomingSequence;        // the last received packet sequence
        public int acknowledgedSequence;    // the last acknowledged packet (by remote peer)

        public NetChannel(Peer peer, NetConnection connection, int playersCount)
        {
            m_peer = peer;
            m_connection = connection;
            m_players = new List<Player>(playersCount);
        }

        //////////////////////////////////////////////////////////////////////////////

        public NetOutgoingMessage CreateMessage(bool autoRecycle = false)
        {
            NetOutgoingMessage msg = m_peer.CreateMessage();
            if (autoRecycle)
            {
                Application.TimerManager().Schedule(MessageRecycleCallback);
            }
            return msg;
        }

        public void SendMessage(NetOutgoingMessage msg, NetConnection recipient)
        {
            m_peer.SendMessage(msg, recipient);
        }

        public void SendMessage(NetOutgoingMessage msg, IPEndPoint recipient)
        {
            throw new NotImplementedException(); // TODO
        }

        public void RecycleMessage(NetOutgoingMessage msg)
        {
            m_peer.RecycleMessage(msg);
        }

        private void MessageRecycleCallback(Timer timer)
        {
            NetOutgoingMessage msg = timer.UserData<NetOutgoingMessage>();
            m_peer.RecycleMessage(msg);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public List<Player> players
        {
            get { return m_players; }
        }

        public Peer peer
        {
            get { return m_peer; }
        }

        public NetConnection connection
        {
            get { return m_connection; }
        }

        #endregion
    }
}
