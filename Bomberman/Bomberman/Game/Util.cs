using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Game
{
    public class Util
    {
        public static int Px2Cx(float points)
        {
            return (int)(points / Constant.CELL_WIDTH);
        }

        public static int Py2Cy(float points)
        {
            return (int)(points / Constant.CELL_HEIGHT);
        }

        public static float Cx2Px(int cells)
        {
            return (cells + 0.5f) * Constant.CELL_WIDTH;
        }

        public static float Cy2Py(int cells)
        {
            return (cells + 0.5f) * Constant.CELL_HEIGHT;
        }
    }
}
