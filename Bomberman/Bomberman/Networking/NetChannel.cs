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
    public class NetChannel : IResettable
    {
        private List<Player> m_players;     // players for this channel
        private NetConnection m_connection; // network connection for this channel

        public int outgoingSequence;        // the last sent packet sequence
        public int incomingSequence;        // the last received packet sequence
        public int acknowledgedSequence;    // the last acknowledged packet (by remote peer)

        public NetChannel(NetConnection connection, List<Player> players)
        {
            m_connection = connection;
            m_players = players;
        }

        //////////////////////////////////////////////////////////////////////////////

        // TODO: refactor these methods

        public void Reset()
        {
            IsReady = false;
            acknowledgedSequence = 0;
            incomingSequence = 0;
            needsFieldState = true;
            needsRoundResults = true;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public List<Player> players
        {
            get { return m_players; }
        }

        public NetConnection connection
        {
            get { return m_connection; }
        }

        // TODO: refactor these properties

        public bool IsReady { get; set; }
        public bool needsFieldState { get; set; }
        public bool needsRoundResults { get; set; }

        #endregion
    }
}
