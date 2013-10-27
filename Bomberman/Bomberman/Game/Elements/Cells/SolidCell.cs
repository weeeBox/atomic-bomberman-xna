using Bomberman.Game.Elements.Fields;

namespace Bomberman.Game.Elements.Cells
{
    public class SolidCell : FieldCell
    {
        public SolidCell(int cx, int cy)
            : base(FieldCellType.Solid, cx, cy)
        {
        }

        public override bool IsObstacle()
        {
            return true;
        }

        public override bool IsSolid()
        {
            return true;
        }

        public override SolidCell AsSolid()
        {
            return this;
        }
    }
}
