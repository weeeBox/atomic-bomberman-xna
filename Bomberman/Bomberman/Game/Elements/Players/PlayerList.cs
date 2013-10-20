using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using BomberEngine.Debugging;
using BomberEngine.Core.Events;

namespace Bomberman.Game.Elements.Players
{
    public class PlayerList : BaseObject, IResettable
    {
        private List<Player> m_list;
        private TimerManager m_timerManager;

        public PlayerList(TimerManager timerManager, int capacity)
        {
            m_timerManager = timerManager;
            m_list = new List<Player>(capacity);
        }

        public void Reset()
        {
            for (int i = 0; i < m_list.Count; ++i)
            {
                m_list[i].Reset();
            }
        }

        public void Add(Player player)
        {
            Debug.Assert(!m_list.Contains(player));
            m_list.Add(player);
        }

        public void Remove(Player player)
        {
            Remove(player, true);
        }

        public void Kill(Player player)
        {
            player.Kill();
            Remove(player, false);
        }

        private void Remove(Player player, bool removeFromList)
        {
            Debug.Assert(m_list.Contains(player));
            if (removeFromList)
            {
                m_list.Remove(player);
            }
        }

        public Player TryGet(int playerIndex)
        {
            if (playerIndex >= 0 && playerIndex < m_list.Count)
            {
                return m_list[playerIndex];
            }

            return null;
        }

        public Player Get(int playerIndex)
        {
            return m_list[playerIndex];
        }

        public int GetCount()
        {
            return m_list.Count;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public int GetAlivePlayerCount()
        {
            int count = 0;
            for (int i = 0; i < m_list.Count; ++i)
            {
                if (m_list[i].IsAlive)
                {
                    ++count;
                }
            }

            return count;
        }

        public bool AllPlayersAreReady()
        {
            for (int i = 0; i < m_list.Count; ++i)
            {
                if (!m_list[i].IsReady)
                {
                    return false;
                }
            }

            return true;
        }

        public void SetPlayersReady(bool ready)
        {
            for (int i = 0; i < m_list.Count; ++i)
            {
                m_list[i].IsReady = ready;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Array

        public int GetAlivePlayers(Player[] array, Player ignoredPlayer = null)
        {
            int count = 0;
            for (int i = 0; i < m_list.Count; ++i)
            {
                Player player = m_list[i];
                if (player != ignoredPlayer && player.IsAlive)
                {
                    array[count] = player;
                    ++count;
                }
            }

            return count;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        private void ScheduleTimer(TimerCallback callback, float delay = 0.0f)
        {
            m_timerManager.Schedule(callback, delay);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public List<Player> list
        {
            get { return m_list; }
        }

        #endregion
    }
}
