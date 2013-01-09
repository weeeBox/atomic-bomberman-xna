using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BombermanLive.Game.Elements.Cells
{
    public class MovingCell : FieldCell
    {
        private Direction direction;

        /* Points coordinates */
        private float px;
        private float py;

        /* Points speed */
        private float speed;

        public MovingCell(int x, int y)
            : base(x, y)
        {

        }

        public float GetPx()
        {
            return px;
        }

        public float GetPy()
        {
            return py;
        }
    }
}
