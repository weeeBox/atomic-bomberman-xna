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
    public class FieldCell : BaseObject, IUpdatable, IResettable, IDestroyable
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

        public FieldCell(FieldCellType type, int cx, int cy)
        {
            this.type = type;

            slotIndex = -1;
            SetCell(cx, cy);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region IResettable

        public virtual void Reset()
        {   
            m_px = m_py = 0.0f;
            m_oldPx = m_oldPy = 0.0f;
            slotIndex = -1;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region IDestroyable

        public virtual void Destroy()
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region IUpdatable

        public virtual void Update(float delta)
        {   
        }

        public virtual void UpdateAnimation(float delta)
        {
        }

        public virtual void UpdateDumb(float delta)
        {
            UpdateAnimation(delta);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Cell

        public void SetCell()
        {
            SetCell(cx, cy);
        }

        public void SetCell(int cx, int cy)
        {
            ForcePos(Util.Cx2Px(cx), Util.Cy2Py(cy));
        }

        public void ForcePos(float px, float py)
        {
            SetPos(px, py);
            oldPx = px;
            oldPy = py;
        }
        
        public virtual void SetPos(float px, float py)
        {
            this.m_px = px;
            this.m_py = py;
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

        public FieldCellSlot GetNearSlot(int dcx, int dcy)
        {
            return GetField().GetSlot(cx + dcx, cy + dcy);
        }

        public FieldCellSlot GetNearSlot(Direction dir)
        {
            switch (dir)
            {
                case Direction.DOWN:
                    return GetNearSlot(0, 1);
                case Direction.UP:
                    return GetNearSlot(0, -1);
                case Direction.LEFT:
                    return GetNearSlot(-1, 0);
                case Direction.RIGHT:
                    return GetNearSlot(1, 0);
                default:
                    Debug.Assert(false, "Unknown dir: " + dir);
                    break;
            }

            return null;
        }

        public bool HasNearObstacle(Direction dir)
        {
            FieldCellSlot slot = GetNearSlot(dir);
            return slot == null || slot.ContainsObstacle();
        }

        public bool HasNearObstacle(int dcx, int dcy)
        {
            FieldCellSlot slot = GetNearSlot(dcx, dcy);
            return slot == null || slot.ContainsObstacle();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Virtual Type

        public virtual bool IsSolid()
        {
            return false;
        }

        public virtual SolidCell AsSolid()
        {
            return null;
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

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Collider

        /* Checks if objects collide */
        public virtual bool Collides(FieldCell other)
        {
            return CheckBounds2BoundsCollision(other)
                || CheckBounds2CellCollision(other)
                || CheckCell2BoundsCollision(other);
        }

        /* Checks cell-to-cell collision: objects collide only if cells collide */
        public bool CheckCell2CellCollision(FieldCell other)
        {
            return cx == other.cx && cy == other.cy;
        }

        /* Checks bounds-to-cell collision: objects collide if caller's cell collides with callee's
         * bounding box */
        public bool CheckBounds2CellCollision(FieldCell other)
        {
            return Math.Abs(px - other.CellCenterPx()) < Constant.CELL_WIDTH && Math.Abs(py - other.CellCenterPy()) < Constant.CELL_HEIGHT;
        }

        /* Checks cell-to-bounds collision: objects collide if caller's bounding box collides with callee's
         * cell */
        public bool CheckCell2BoundsCollision(FieldCell other)
        {
            return other.CheckBounds2CellCollision(this);
        }

        /* Checks bounds-to-bounds collision: objects collide if bounding boxes collide */
        public bool CheckBounds2BoundsCollision(FieldCell other)
        {
            return Math.Abs(px - other.px) < Constant.CELL_WIDTH && Math.Abs(py - other.py) < Constant.CELL_HEIGHT;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region TimerManager

        protected Timer ScheduleTimer(TimerCallback2 callback, float delay = 0.0f, bool repeated = false)
        {
            return GetField().ScheduleTimer(callback, delay, repeated);
        }

        protected Timer ScheduleTimerOnce(TimerCallback2 callback, float delay = 0.0f, bool repeated = false)
        {
            return GetField().ScheduleTimerOnce(callback, delay, repeated);
        }

        protected void CancelTimer(TimerCallback2 callback)
        {
            GetField().CancelTimer(callback);
        }

        protected void CancelAllTimers()
        {
            GetField().CancelAllTimers(this);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

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

        public float OverlapX(FieldCell other)
        {
            return OverlapX(this, other);
        }

        public float OverlapY(FieldCell other)
        {
            return OverlapY(this, other);
        }

        public static float OverlapX(FieldCell a, FieldCell b)
        {
            float overlapX = Constant.CELL_WIDTH - Math.Abs(a.px - b.px);
            return overlapX > 0 ? overlapX : 0;
        }

        public static float OverlapY(FieldCell a, FieldCell b)
        {
            float overlapY = Constant.CELL_HEIGHT - Math.Abs(a.py - b.py);
            return overlapY > 0 ? overlapY : 0;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

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

        #endregion
    }
}
