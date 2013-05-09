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

        public void KeyPressed(Keys key)
        {
            foreach (IKeyboardListener l in listeners)
            {
                l.KeyPressed(key);
            }
        }

        public void KeyReleased(Keys key)
        {
            foreach (IKeyboardListener l in listeners)
            {
                l.KeyReleased(key);
            }
        }
    }
}
