using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements.Items;
using Bomberman.Game.Elements.Players.Input;

namespace Bomberman.Game.Elements.Players
{
    public class Player : MovingCell
    {
        private static readonly int SPEED_START = 145;
        private static readonly int SPEED_INCREASE = 25;

        private bool alive;

        private int bombRadius;
        private float bombTimeout;
        private bool bombBouncing;
        private bool bombDetonated;

        private PlayerInput input;

        public Player(int index, int x, int y)
            : base(x, y)
        {
            alive = true;
            direction = Direction.DOWN;
        }

        public override void Update(float delta)
        {
        }

        public bool IsAlive()
        {
            return alive;
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
