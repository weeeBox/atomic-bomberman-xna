using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core.Input;

namespace BomberEngine.Core.Events
{
    public enum KeyState
    {
        Pressed,
        Repeated,
        Released
    }

    public class KeyEvent : Event
    {
        public KeyEventArg arg;
        public KeyState state;

        public KeyEvent()
            : base(Event.KEY)
        {
        }

        public KeyEvent Init(KeyEventArg arg, KeyState state)
        {
            this.arg = arg;
            this.state = state;

            return this;
        }
    }
}
