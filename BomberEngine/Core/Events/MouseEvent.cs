using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Events
{
    public class MouseEvent : Event
    {
        public const int PRESSED = 1;
        public const int DRAGGED = 2;
        public const int RELEASED = 3;

        public int x;
        public int y;
        public int state;

        public MouseEvent()
            : base(Event.MOUSE)
        {
        }
    }
}
