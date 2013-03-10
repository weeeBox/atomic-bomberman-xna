using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Debugging;
using Bomberman.Game.Elements.Cells;
using Bomberman.Game.Elements.Players;
using BomberEngine.Util;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldCell : ListNode<FieldCell>, Updatable
    {
        public FieldCellType type;

        /* Coordinates in points */
        private float m_px;
        private float m_py;

        /* Position on previous tick */
        private float m_oldPx;
        private float m_oldPy;

        /* Linked list stuff */
        public int slotIndex;

        /* Indicates if can be used */
        public bool valid;

        public FieldCell(FieldCellType type, int cx, int cy)
        {
            this.type = type;

            slotIndex = -1;
            valid = true;
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
            oldPx = px;
            oldPy = py;
        }
        
        public virtual void SetPos(float px, float py)
        {
            this.m_px = px;
            this.m_py = py;
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

        public FieldCellSlot NearSlot(int dcx, int dcy)
        {
            return GetField().GetSlot(cx + dcx, cy + dcy);
        }

        public FieldCellSlot NearSlotDir(Direction dir)
        {
            switch (dir)
            {
                case Direction.DOWN:
                    return NearSlot(0, 1);
                case Direction.UP:
                    return NearSlot(0, -1);
                case Direction.LEFT:
                    return NearSlot(-1, 0);
                case Direction.RIGHT:
                    return NearSlot(1, 0);
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

        protected FieldCellSlot GetSlot(int cx, int cy)
        {
            return GetField().GetSlot(cx, cy);
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
            get { return m_oldPx; }
            protected set { m_oldPx = value; }
        }

        public float oldPy
        {
            get { return m_oldPy; }
            protected set { m_oldPy = value; }
        }

        public float moveDx
        {
            get { return m_px - m_oldPx; }
        }

        public float moveDy
        {
            get { return m_py - m_oldPy; }
        }

        public int GetPriority()
        {
            return (int)type;
        }

        public static float OverlapX(FieldCell a, FieldCell b)
        {
            float overlapX = Constant.CELL_WIDTH - MathHelp.Abs(a.px - b.px);
            return overlapX > 0 ? overlapX : 0;
        }

        public static float OverlapY(FieldCell a, FieldCell b)
        {
            float overlapY = Constant.CELL_HEIGHT - MathHelp.Abs(a.py - b.py);
            return overlapY > 0 ? overlapY : 0;
        }
    }
}
