using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BombermanCommon.Resources.Scheme
{
    public struct PlayerInfo
    {
        public int x;
        public int y;
        public int team;

        public PlayerInfo(int x, int y, int team)
        {
            this.x = x;
            this.y = y;
            this.team = team;
        }
    }
}
