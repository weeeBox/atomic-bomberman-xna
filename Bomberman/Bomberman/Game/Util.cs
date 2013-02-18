using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Elements;
using BomberEngine.Debugging;
using BomberEngine.Util;

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

        public static float CellCenterPx(float px)
        {
            return Cx2Px(Px2Cx(px));
        }
        
        public static float CellCenterPy(float py)
        {
            return Cy2Py(Py2Cy(py));
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

        public static void ShuffleArray<T>(T[] array)
        {
            ShuffleArray(array, array.Length);
        }

        public static void ShuffleArray<T>(T[] array, int size)
        {  
            int n = size;
            while (n > 1)
            {
                n--;
                int k = MathHelp.NextInt(n + 1);
                T value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }
    }
}
