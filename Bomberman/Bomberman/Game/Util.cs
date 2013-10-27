using System;
using BomberEngine;
using Bomberman.Game.Elements;

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

        public static float CellLeftX(float px)
        {
            return CellCenterPx(px) - 0.5f * ch;
        }

        public static float CellRightX(float px)
        {
            return CellCenterPx(px) + 0.5f * ch;
        }

        public static float CellTopY(float py)
        {
            return CellCenterPy(py) - 0.5f * ch;
        }

        public static float CellBottomY(float py)
        {
            return CellCenterPy(py) + 0.5f * ch;
        }

        public static float CellCenterPx(float px)
        {
            return Cx2Px(Px2Cx(px));
        }
        
        public static float CellCenterPy(float py)
        {
            return Cy2Py(Py2Cy(py));
        }

        public static float CellCenterOffX(float px)
        {
            return px - CellCenterPx(px);
        }

        public static float CellCenterOffY(float py)
        {
            return py - CellCenterPy(py);
        }

        public static float TargetPxOffset(float px, float k = 1.0f)
        {
            float offset = CellCenterOffX(px);
            return Math.Abs(offset) < k * 0.5f * cw ? -offset : Math.Sign(offset) * (cw - offset);
        }

        public static float TargetPyOffset(float py, float k = 1.0f)
        {
            float offset = CellCenterOffY(py);
            return Math.Abs(offset) < k * 0.5f * ch ? -offset : Math.Sign(offset) * (ch - offset);
        }

        public static float TravelDistanceX(float fromPx, int toCx)
        {
            int fromCx = Px2Cx(fromPx);
            if (fromCx != toCx)
            {
                return cw * (toCx - fromCx) + (CellCenterPx(fromPx) - fromPx);
            }
            return 0;
        }

        public static float TravelDistanceY(float fromPy, int toCy)
        {   
            int fromCy = Py2Cy(fromPy);
            if (fromCy != toCy)
            {
                return ch * (toCy - fromCy) + (CellCenterPy(fromPy) - fromPy);
            }

            return 0;
        }

        public static bool AreOpposite(Direction d1, Direction d2)
        {
            return d1 == Opposite(d2);
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
