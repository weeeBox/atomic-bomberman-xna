using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;

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

        public virtual bool IsBreakable()
        {
            return false;
        }

        public virtual bool IsObstacle()
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
    }
}
