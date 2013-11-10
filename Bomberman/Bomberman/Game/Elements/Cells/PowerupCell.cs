using Bomberman.Gameplay.Elements.Fields;

namespace Bomberman.Gameplay.Elements.Cells
{
    public class PowerupCell : FieldCell
    {
        public int powerup;
        public float elasped;

        public PowerupCell(int
            powerup, int cx, int cy)
            : base(FieldCellType.Powerup, cx, cy)
        {
            this.powerup = powerup;
        }

        public override void Update(float delta)
        {
            elasped += delta;
        }

        public override PowerupCell AsPowerup()
        {
            return this;
        }

        public override bool IsPowerup()
        {
            return true;
        }
    }
}
