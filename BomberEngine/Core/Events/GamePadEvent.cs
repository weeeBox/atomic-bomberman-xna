using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Events
{
    public class GamePadEvent : Event
    {
        public const int DISCONNECTED = 0;
        public const int CONNECTED = 1;

        public int index;
        public int state;

        public GamePadEvent()
            : base(Event.GAMEPAD)
        {
        }
    }
}
