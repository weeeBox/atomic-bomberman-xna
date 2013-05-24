using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Events
{
    public class Event
    {
        public const int KEYBOARD = 1;
        public const int GAMEPAD = 2;
        public const int GAMEPAD_CONNECTIVITY = 3;
        public const int MOUSE = 4;

        public int code;

        public Event(int code)
        {
            this.code = code;
        }
    }
}
