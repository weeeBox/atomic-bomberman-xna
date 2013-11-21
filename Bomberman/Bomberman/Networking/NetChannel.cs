using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Gameplay.Elements.Players;
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

        private bool m_ready;

        public NetChannel(NetConnection connection, Player player)            
        {
            m_connection = connection;

            m_players = new List<Player>(1);
            m_players.Add(player);

            player.NetChannel = this;

            Reset();
        }

        public NetChannel(NetConnection connection, List<Player> players)
        {
            m_connection = connection;
            m_players = players;

            SetChannel(players);
            Reset();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region IResettable

        public void Reset()
        {
            IsReady                 = false;
            outgoingSequence        = -1;
            incomingSequence        = -1;
            acknowledgedSequence    = -1;
            needsFieldState         = true;
            needsRoundResults       = true;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        private void SetChannel(List<Player> players)
        {
            for (int i = 0; i < players.Count; ++i)
            {
                players[i].NetChannel = this;
            }
        }

        #endregion

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

        public bool IsReady 
        {
            get { return m_ready; }
            set 
            {
                m_ready = value;
                for (int i = 0; i < m_players.Count; ++i)
                {
                    m_players[i].IsReady = value;
                }
            }
        }
        public bool needsFieldState { get; set; }
        public bool needsRoundResults { get; set; }

        #endregion
    }
}
