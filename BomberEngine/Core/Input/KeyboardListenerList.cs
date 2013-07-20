using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Util;
using BomberEngine.Debugging;

namespace BomberEngine.Core.Input
{
    public class IKeyInputListenerList : BaseList<IKeyInputListener>, IKeyInputListener
    {   
        public IKeyInputListenerList()
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
}
