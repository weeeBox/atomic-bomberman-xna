using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements.Items;

namespace Bomberman.Game.Elements.Players
{
    public class Player : MovingCell
    {
        private int bombRadius;
        private float bombTimeout;
        private bool bombBouncing;
        private bool bombDetonated;

        public Player(int index, int x, int y)
            : base(x, y)
        {
        }

        public Bomb SetBomb()
        {
            return new Bomb(this, false); // TODO: calculate dud
        }

        public float GetBombTimeout()
        {
            return bombTimeout;
        }

        public int GetBombRadius()
        {
            return bombRadius;
        }

        public bool IsBombBouncing()
        {
            return bombBouncing;
        }

        public bool IsBobmDetonated()
        {
            return bombDetonated;
        }
    }
}
