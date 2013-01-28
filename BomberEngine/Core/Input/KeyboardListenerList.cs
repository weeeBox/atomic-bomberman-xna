using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Util;

namespace BomberEngine.Core.Input
{
    public class KeyboardListenerList : KeyboardListener
    {
        private ConcurrentList<KeyboardListener> listeners;

        public KeyboardListenerList()
        {
            listeners = new ConcurrentList<KeyboardListener>();
        }

        public bool Add(KeyboardListener listener)
        {
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
                return true;
            }
            return false;
        }

        public bool Remove(KeyboardListener listener)
        {
            return listeners.Remove(listener);
        }

        public void KeyPressed(Keys key)
        {
            foreach (KeyboardListener l in listeners)
            {
                l.KeyPressed(key);
            }
        }

        public void KeyReleased(Keys key)
        {
            foreach (KeyboardListener l in listeners)
            {
                l.KeyReleased(key);
            }
        }
    }
}
