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
    public class PlayerList : IResettable
    {
        public List<Player> list;

        private TimerManager timerManager;

        public PlayerList(TimerManager timerManager, int capacity)
        {
            this.timerManager = timerManager;
            list = new List<Player>(capacity);
        }

        public void Reset()
        {
            for (int i = 0; i < list.Count; ++i)
            {
                list[i].Reset();
            }
        }

        public void Add(Player player)
        {
            Debug.Assert(!list.Contains(player));
            list.Add(player);
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
            Debug.Assert(list.Contains(player));
            if (removeFromList)
            {
                list.Remove(player);
            }
        }

        public Player TryGet(int playerIndex)
        {
            if (playerIndex >= 0 && playerIndex < list.Count)
            {
                return list[playerIndex];
            }

            return null;
        }

        public Player Get(int playerIndex)
        {
            return list[playerIndex];
        }

        public int GetCount()
        {
            return list.Count;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public int GetAlivePlayerCount()
        {
            int count = 0;
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].IsAlive())
                {
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
            timerManager.Schedule(callback, delay);
        }

        #endregion
    }
}
