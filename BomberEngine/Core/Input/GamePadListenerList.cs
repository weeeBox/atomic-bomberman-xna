using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;

namespace BomberEngine.Core.Input
{
    public class GamePadListenerList : GamePadListener
    {
        private ConcurrentList<GamePadListener> listeners;

        public GamePadListenerList()
        {
            listeners = new ConcurrentList<GamePadListener>();
        }

        public bool Add(GamePadListener listener)
        {
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
                return true;
            }
            return false;
        }

        public bool Remove(GamePadListener listener)
        {
            return listeners.Remove(listener);
        }

        public void ButtonPressed(ButtonEvent e)
        {
            foreach (GamePadListener l in listeners)
            {
                l.ButtonPressed(e);
            }
        }

        public void ButtonReleased(ButtonEvent e)
        {
            foreach (GamePadListener l in listeners)
            {
                l.ButtonReleased(e);
            }
        }
    }
}
