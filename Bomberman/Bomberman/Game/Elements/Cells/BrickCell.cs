using Bomberman.Gameplay.Elements.Fields;

namespace Bomberman.Gameplay.Elements.Cells
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
                GetField().ScheduleTimer(DestroyCallback, 0.5f);
            }
        }

        private void DestroyCallback()
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

        //////////////////////////////////////////////////////////////////////////////

        #region Equality

        public override bool EqualsTo(FieldCell other)
        {
            BrickCell brick = other as BrickCell;
            return base.EqualsTo(brick) && brick.powerup == powerup;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////
    }
}
