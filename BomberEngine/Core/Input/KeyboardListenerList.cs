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
        private ObjectsList<KeyboardListener> listeners;

        public KeyboardListenerList()
        {
            listeners = new ObjectsList<KeyboardListener>();
        }

        public void Add(KeyboardListener listener)
        {
            listeners.Add(listener);
        }

        public void Remove(KeyboardListener listener)
        {
            listeners.Remove(listener);
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
