using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core.Input;

namespace BomberEngine.Core.Events
{
    public class KeyEvent : Event
    {
        public const int PRESSED = 1;
        public const int REPEATED = 2;
        public const int RELEASED = 3;

        public KeyEventArg arg;
        public int state;

        public KeyEvent()
            : base(Event.KEY)
        {
        }

        public KeyEvent Init(KeyEventArg arg, int state)
        {
            this.arg = arg;
            this.state = state;

            return this;
        }
    }
}
