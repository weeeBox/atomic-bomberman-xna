using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Util;

namespace BomberEngine.Core.Input
{
    public class KeyboardListenerList : IKeyboardListener
    {
        private ConcurrentList<IKeyboardListener> listeners;

        public KeyboardListenerList()
        {
            listeners = new ConcurrentList<IKeyboardListener>();
        }

        public bool Add(IKeyboardListener listener)
        {
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
                return true;
            }
            return false;
        }

        public bool Remove(IKeyboardListener listener)
        {
            return listeners.Remove(listener);
        }

        public bool OnKeyPressed(Keys key)
        {
            bool handled = false;
            foreach (IKeyboardListener l in listeners)
            {
                handled |= l.OnKeyPressed(key);
            }
            return handled;
        }

        public bool OnKeyReleased(Keys key)
        {
            bool handled = false;
            foreach (IKeyboardListener l in listeners)
            {
                handled |= l.OnKeyReleased(key);
            }

            return handled;
        }
    }
}
