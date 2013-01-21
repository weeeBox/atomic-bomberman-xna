using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements;
using BomberEngine.Debugging;

namespace Bomberman.Game
{
    public class Util
    {
        private static readonly int cw = Constant.CELL_WIDTH;
        private static readonly int ch = Constant.CELL_HEIGHT;

        public static int Px2Cx(float px)
        {
            return (int)(px / cw);
        }

        public static int Py2Cy(float py)
        {
            return (int)(py / ch);
        }

        public static float Cx2Px(int cx)
        {
            return (cx + 0.5f) * cw;
        }

        public static float Cy2Py(int cy)
        {
            return (cy + 0.5f) * ch;
        }

        public static float TargetPxOffset(float px)
        {
            float prevPx = Cx2Px(Px2Cx(px));
            float dx = px - prevPx;
            return dx < 0.5f * cw ? -dx : (cw - dx);
        }

        public static float TargetPyOffset(float py)
        {
            float prevPy = Cy2Py(Py2Cy(py));
            float dy = py - prevPy;
            return dy < 0.5f * ch ? -dy : (ch - dy);
        }

        public static Direction Opposite(Direction direction)
        {
            switch (direction)
            {
                case Direction.DOWN:
                    return Direction.UP;
                case Direction.UP:
                    return Direction.DOWN;
                case Direction.LEFT:
                    return Direction.RIGHT;
                case Direction.RIGHT:
                    return Direction.LEFT;
                default:
                    Debug.Assert(false, "Unknown direction: " + direction);
                    break;
            }

            return Direction.DOWN;
        }
    }
}
