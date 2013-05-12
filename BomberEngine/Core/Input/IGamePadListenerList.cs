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

        public void OnButtonPressed(ButtonEvent e)
        {
            foreach (IGamePadListener l in listeners)
            {
                l.OnButtonPressed(e);
            }
        }

        public void OnButtonReleased(ButtonEvent e)
        {
            foreach (IGamePadListener l in listeners)
            {
                l.OnButtonReleased(e);
            }
        }
    }
}
