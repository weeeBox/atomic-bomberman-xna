using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Game.Multiplayer
{
    public class ClientGamePacket : GamePacket
    {
        public int actionStatesBitmask;
        public bool firstRun;
    }
}
