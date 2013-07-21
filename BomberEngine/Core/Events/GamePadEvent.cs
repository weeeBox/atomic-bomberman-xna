using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Events
{
    public enum GamePadState
    {
        Disconnected,
        Connected
    }

    public class GamePadEvent : Event
    {
        public int index;
        public GamePadState state;

        public GamePadEvent()
            : base(Event.GAMEPAD)
        {   
        }

        public GamePadEvent Init(int index, GamePadState state)
        {
            this.index = index;
            this.state = state;

            return this;
        }
    }
}
