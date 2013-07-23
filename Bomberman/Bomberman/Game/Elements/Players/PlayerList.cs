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
    public class PlayerList : IEventHandler, IResettable
    {
        public List<Player> list;

        private TimerManager timerManager;
        private KeyInputListenerList keyListeners;

        public PlayerList(TimerManager timerManager, int capacity)
        {
            this.timerManager = timerManager;
            list = new List<Player>(capacity);
            
            keyListeners = new KeyInputListenerList(capacity);
        }

        public void Reset()
        {
            keyListeners.Clear();
            
            for (int i = 0; i < list.Count; ++i)
            {
                Player p = list[i];

                p.Reset();
                if (p.input is IKeyInputListener)
                {
                    keyListeners.Add(p.input as IKeyInputListener);
                }
            }
        }

        public void Add(Player player)
        {
            Debug.Assert(!list.Contains(player));
            list.Add(player);

            if (player.input is IKeyInputListener)
            {
                keyListeners.Add(player.input as IKeyInputListener);
            }
        }

        public void Remove(Player player)
        {
            Remove(player, true);
        }

        public void Kill(Player player)
        {
            player.Kill();
            Remove(player, false);
            ScheduleTimer(player.DeathTimerCallback, 3.0f);
        }

        private void Remove(Player player, bool removeFromList)
        {
            Debug.Assert(list.Contains(player));
            if (removeFromList)
            {
                list.Remove(player);
            }

            if (player.input is IKeyInputListener)
            {
                keyListeners.Remove(player.input as IKeyInputListener);
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

        #region IEventHandler

        public bool HandleEvent(Event evt)
        {
            if (evt.code == Event.KEY)
            {
                KeyEvent keyEvent = (KeyEvent)evt;
                if (keyEvent.state == KeyState.Pressed)
                {
                    return OnKeyPressed(keyEvent.arg);
                }

                if (keyEvent.state == KeyState.Repeated)
                {
                    return OnKeyRepeated(keyEvent.arg);
                }

                if (keyEvent.state == KeyState.Released)
                {
                    return OnKeyReleased(keyEvent.arg);
                }

                return false;
            }

            return false;
        }

        private bool OnKeyPressed(KeyEventArg arg)
        {
            return keyListeners.OnKeyPressed(arg);
        }

        private bool OnKeyRepeated(KeyEventArg arg)
        {
            return keyListeners.OnKeyRepeated(arg);
        }

        private bool OnKeyReleased(KeyEventArg arg)
        {
            return keyListeners.OnKeyReleased(arg);
        }

        #endregion

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
