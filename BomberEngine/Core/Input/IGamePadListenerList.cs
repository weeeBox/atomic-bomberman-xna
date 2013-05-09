using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;

namespace BomberEngine.Core.Input
{
    public class IGamePadListenerList : IGamePadListener
    {
        private ConcurrentList<IGamePadListener> listeners;

        public IGamePadListenerList()
        {
            listeners = new ConcurrentList<IGamePadListener>();
        }

        public bool Add(IGamePadListener listener)
        {
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
                return true;
            }
            return false;
        }

        public bool Remove(IGamePadListener listener)
        {
            return listeners.Remove(listener);
        }

        public void ButtonPressed(ButtonEvent e)
        {
            foreach (IGamePadListener l in listeners)
            {
                l.ButtonPressed(e);
            }
        }

        public void ButtonReleased(ButtonEvent e)
        {
            foreach (IGamePadListener l in listeners)
            {
                l.ButtonReleased(e);
            }
        }
    }
}
