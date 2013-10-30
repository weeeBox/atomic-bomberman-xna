using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Elements.Cells
{
    public class BrickCell : FieldCell
    {
        public int powerup;
        public bool hit;

        public BrickCell(int cx, int cy)
            : base(FieldCellType.Brick, cx, cy)
        {
            powerup = Powerups.None;
        }

        public void Hit()
        {
            if (!hit)
            {
                hit = true;
                GetField().ScheduleTimer(Destroy, 0.5f);
            }
        }

        private void Destroy()
        {
            GetField().DestroyBrick(this);
        }

        public bool HasPowerup()
        {
            return powerup != Powerups.None;
        }

        public override BrickCell AsBrick()
        {
            return this;
        }

        public override bool IsBrick()
        {
            return true;
        }

        public override bool IsObstacle()
        {
            return true;
        }
    }
}
