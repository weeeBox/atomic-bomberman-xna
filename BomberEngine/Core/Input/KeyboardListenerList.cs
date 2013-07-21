using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Util;
using BomberEngine.Debugging;

namespace BomberEngine.Core.Input
{
    public class KeyInputListenerList : BaseList<IKeyInputListener>, IKeyInputListener
    {
        private static readonly IKeyInputListener nullListener = new NullKeyInputListener();

        public KeyInputListenerList(int capacity)
            : base(nullListener, capacity)
        {   
        }

        protected KeyInputListenerList(List<IKeyInputListener> list, IKeyInputListener nullListener)
            : base(list, nullListener)
        {
        }

        public override bool Add(IKeyInputListener listener)
        {
            Debug.Assert(!list.Contains(listener));
            return base.Add(listener);
        }

        public override bool Remove(IKeyInputListener listener)
        {
            Debug.Assert(list.Contains(listener));
            return base.Remove(listener);
        }

        public bool OnKeyPressed(KeyEventArg arg)
        {
            bool handled = false;
            int count = list.Count;
            for (int i = 0; i < count; ++i)
            {
                handled |= list[i].OnKeyPressed(arg);
            }

            ClearRemoved();
            return handled;
        }

        public bool OnKeyRepeated(KeyEventArg arg)
        {
            bool handled = false;
            int count = list.Count;
            for (int i = 0; i < count; ++i)
            {
                handled |= list[i].OnKeyRepeated(arg);
            }

            ClearRemoved();
            return handled;
        }

        public bool OnKeyReleased(KeyEventArg arg)
        {
            bool handled = false;
            int count = list.Count;
            for (int i = 0; i < count; ++i)
            {
                handled |= list[i].OnKeyReleased(arg);
            }

            ClearRemoved();
            return handled;
        }
    }

    internal class NullKeyInputListener : IKeyInputListener
    {
        public bool OnKeyPressed(KeyEventArg arg)
        {
            return false;
        }

        public bool OnKeyRepeated(KeyEventArg arg)
        {
            return false;
        }

        public bool OnKeyReleased(KeyEventArg arg)
        {
            return false;
        }
    }
}