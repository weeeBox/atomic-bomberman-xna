using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Debugging;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements.Players;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldCell : Updatable
    {
        /* Coordinates in points */
        private float m_px;
        private float m_py;

        /* Last move distance */
        private float m_moveDx;
        private float m_moveDy;

        /* Linked list stuff */
        public FieldCell listNext;
        public FieldCell listPrev;
        public int listIndex;

        /* Priority inside the list. Cells with higher priority come first */
        public int listPriority;

        public FieldCell(int cx, int cy)
        {
            listIndex = -1;
            SetCell(cx, cy);
        }

        public virtual void Update(float delta)
        {   
        }

        public void SetCell()
        {
            SetCell(cx, cy);
        }

        public void SetCell(int cx, int cy)
        {
            SetPos(Util.Cx2Px(cx), Util.Cy2Py(cy));
        }
        
        public virtual void SetPos(float px, float py)
        {
            this.m_px = px;
            this.m_py = py;
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

        public virtual BrickCell AsBrick()
        {
            return null;
        }

        public virtual bool IsObstacle()
        {
            return false;
        }

        public virtual bool IsMovable()
        {
            return false;
        }

        public virtual MovableCell AsMovable()
        {
            return null;
        }

        public virtual bool IsBomb()
        {
            return false;
        }

        public virtual Bomb AsBomb()
        {
            return null;
        }

        public virtual bool IsPlayer()
        {
            return false;
        }

        public virtual Player AsPlayer()
        {
            return null;
        }

        public virtual bool IsFlame()
        {
            return false;
        }

        public virtual FlameCell AsFlame()
        {
            return null;
        }

        public virtual bool IsPowerup()
        {
            return false;
        }

        public virtual PowerupCell AsPowerup()
        {
            return null;
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

        public float CellCenterPx()
        {
            return Util.Cx2Px(cx);
        }

        public float CellCenterPy()
        {
            return Util.Cy2Py(cy);
        }

        public float CenterOffsetX()
        {
            return px - CellCenterPx();
        }

        public float CenterOffsetY()
        {
            return py - CellCenterPy();
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

        public void RemoveFromField()
        {
            GetField().RemoveCell(this);
        }

        protected Field GetField()
        {
            return Field.Current();
        }

        /* This is a bit tricky: I'm not gonna use properties or any other C# specific
           stuff. Just keep them to check if some code is trying to assign "readonly" public
           field.
         */
        public int cx
        {
            get { return Util.Px2Cx(px); }
        }

        public int cy
        {
            get { return Util.Py2Cy(py); }
        }

        public float px
        {
            get { return m_px; }
        }

        public float py
        {
            get { return m_py; }
        }

        public float oldPx
        {
            get { return px - m_moveDx; }
        }

        public float oldPy
        {
            get { return py - m_moveDy; }
        }

        public float moveDx
        {
            get { return m_moveDx; }
            protected set { m_moveDx = value; }
        }

        public float moveDy
        {
            get { return m_moveDy; }
            protected set { m_moveDy = value; }
        }
    }
}
