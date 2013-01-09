using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BombermanLive.Game.Elements.Cells;

namespace BombermanLive.Game.Elements.Players
{
    public class Player : MovingCell
    {
        Player(int index, int x, int y)
            : base(x, y)
        {
        }
    }
}
