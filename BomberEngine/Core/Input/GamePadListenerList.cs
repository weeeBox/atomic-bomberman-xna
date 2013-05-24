using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;

namespace BomberEngine.Core.Input
{
    public class GamePadListenerList : IGamePadListener
    {
        private ConcurrentList<IGamePadListener> listeners;

        public GamePadListenerList()
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

        public bool OnButtonPressed(ButtonEvent e)
        {
            bool handled = false;
            foreach (IGamePadListener l in listeners)
            {
                handled |= l.OnButtonPressed(e);
            }
            return handled;
        }

        public bool OnButtonRepeat(ButtonEvent e)
        {
            bool handled = false;
            foreach (IGamePadListener l in listeners)
            {
                handled |= l.OnButtonRepeat(e);
            }
            return handled;
        }

        public bool OnButtonReleased(ButtonEvent e)
        {
            bool handled = false;
            foreach (IGamePadListener l in listeners)
            {
                handled |= l.OnButtonReleased(e);
            }
            return handled;
        }
    }
}
