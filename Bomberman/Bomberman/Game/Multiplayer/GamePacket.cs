using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Game.Multiplayer
{
    public abstract class GamePacket
    {
        public int id;
        public int lastAcknowledgedId;
        public float timeStamp;
    }
}
