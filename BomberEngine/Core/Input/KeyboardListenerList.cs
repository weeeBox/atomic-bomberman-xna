using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Util;

namespace BomberEngine.Core.Input
{
    public class IKeyInputListenerList : IKeyInputListener
    {
        private ConcurrentList<IKeyInputListener> listeners;

        public IKeyInputListenerList()
        {
            listeners = new ConcurrentList<IKeyInputListener>();
        }

        public bool Add(IKeyInputListener listener)
        {
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
                return true;
            }
            return false;
        }

        public bool Remove(IKeyInputListener listener)
        {
            return listeners.Remove(listener);
        }

        public bool OnKeyPressed(KeyEventArg arg)
        {
            bool handled = false;
            foreach (IKeyInputListener l in listeners)
            {
                handled |= l.OnKeyPressed(arg);
            }
            return handled;
        }

        public bool OnKeyRepeated(KeyEventArg key)
        {
            bool handled = false;
            foreach (IKeyInputListener l in listeners)
            {
                handled |= l.OnKeyRepeated(key);
            }
            return handled;
        }

        public bool OnKeyReleased(KeyEventArg key)
        {
            bool handled = false;
            foreach (IKeyInputListener l in listeners)
            {
                handled |= l.OnKeyReleased(key);
            }

            return handled;
        }
    }
}
