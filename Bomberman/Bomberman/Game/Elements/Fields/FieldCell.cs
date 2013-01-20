using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Debugging;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldCell : Updatable
    {
        /* Coordinates in cells */
        protected int cx;
        protected int cy;

        /* Coordinates in points */
        protected float px;
        protected float py;

        public FieldCell(int cx, int cy)
        {
            SetCell(cx, cy);
        }

        public virtual void Update(float delta)
        {   
        }

        public void SetCell(int cx, int cy)
        {
            this.cx = cx;
            this.cy = cy;

            px = Util.Cx2Px(cx);
            py = Util.Cy2Py(cy);
        }

        public virtual bool IsEmpty()
        {
            return false;
        }

        public virtual bool IsSolid()
        {
            return false;
        }

        public virtual bool IsBrick()
        {
            return false;
        }

        public virtual bool IsObstacle()
        {
            return false;
        }

        public virtual bool IsBomb()
        {
            return false;
        }

        public virtual bool IsPlayer()
        {
            return false;
        }

        public virtual bool IsExplosion()
        {
            return false;
        }

        public virtual bool IsPowerup()
        {
            return false;
        }

        public int GetCx()
        {
            return cx;
        }

        public int GetCy()
        {
            return cy;
        }

        public float GetPx()
        {
            return px;
        }

        public float GetPy()
        {
            return py;
        }

        public FieldCell NearCell(int dcx, int dcy)
        {
            return GetField().GetCell(cx + dcx, cy + dcy);
        }

        public FieldCell NearCellDir(Direction dir)
        {
            switch (dir)
            {
                case Direction.DOWN:
                    return NearCell(0, 1);
                case Direction.UP:
                    return NearCell(0, -1);
                case Direction.LEFT:
                    return NearCell(-1, 0);
                case Direction.RIGHT:
                    return NearCell(1, 0);
                default:
                    Debug.Assert(false, "Unknown dir: " + dir);
                    break;
            }

            return null;
        }

        protected Field GetField()
        {
            return Field.Current();
        }
    }
}
