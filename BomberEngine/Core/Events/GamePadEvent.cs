using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core.Input;

namespace BomberEngine.Core.Events
{
    public class GamePadEvent : Event
    {
        public const int PRESSED = 1;
        public const int REPEATED = 2;
        public const int RELEASED = 3;

        public ButtonEventArg arg;
        public int state;

        public GamePadEvent()
            : base(Event.GAMEPAD)
        {
        }

        public Event Init(ButtonEventArg arg, int state)
        {
            this.arg = arg;
            this.state = state;

            return this;
        }
    }
}
