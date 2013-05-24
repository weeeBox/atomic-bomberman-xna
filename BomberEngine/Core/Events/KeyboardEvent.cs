using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace BomberEngine.Core.Events
{
    public class KeyboardEvent : Event
    {
        public const int PRESSED = 1;
        public const int REPEATED = 2;
        public const int RELEASED = 3;

        public Keys key;
        public int state;

        public KeyboardEvent()
            : base(Event.KEYBOARD)
        {
        }

        public KeyboardEvent Init(Keys key, int state)
        {
            this.key = key;
            this.state = state;

            return this;
        }
    }
}
