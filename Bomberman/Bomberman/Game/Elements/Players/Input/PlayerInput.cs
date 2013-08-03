using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine;
using BomberEngine.Debugging;
using BomberEngine.Core;

namespace Bomberman.Game.Elements.Players.Input
{
    public abstract class PlayerInput : IUpdatable, IResettable
    {
        private int m_stateBits;
        private int m_stateBitsOld;

        private int m_pressedCount;
        private bool m_active;

        public virtual void Update(float delta)
        {
            m_stateBitsOld = m_stateBits;
        }

        public void Reset()
        {
            m_stateBits = 0;
            m_stateBitsOld = 0;
            m_pressedCount = 0;
            m_active = true;
        }

        protected void SetActionPressed(PlayerAction action, bool flag)
        {
            int index = GetActionIndex(action);
            SetActionPressed(index, flag);
        }

        protected void SetActionPressed(int index, bool flag)
        {
            if (flag)
            {
                m_stateBits |= 1 << index;
                if (IsActionJustPressed(index))
                {
                    ++m_pressedCount;
                }
            }
            else
            {
                m_stateBits &= ~(1 << index);
                if (IsActionJustReleased(index))
                {
                    Debug.Assert(m_pressedCount > 0);
                    --m_pressedCount;
                }
            }
        }
        
        public bool IsActionPressed(PlayerAction action)
        {
            int index = GetActionIndex(action);
            return IsActionPressed(index);
        }

        public bool IsActionJustPressed(PlayerAction action)
        {
            int index = GetActionIndex(action);
            return IsActionJustPressed(index);
        }

        public bool IsActionJustReleased(PlayerAction action)
        {
            int index = GetActionIndex(action);
            return IsActionJustReleased(index);
        }

        public bool IsActionPressed(int index)
        {
            return IsActionPressed(m_stateBits, index);
        }

        public bool IsActionJustPressed(int index)
        {
            return IsActionPressed(m_stateBits, index) && !IsActionPressed(m_stateBitsOld, index);
        }

        public bool IsActionJustReleased(int index)
        {
            return !IsActionPressed(m_stateBits, index) && IsActionPressed(m_stateBitsOld, index);
        }

        private bool IsActionPressed(int bits, int index)
        {
            return (bits & (1 << index)) != 0;
        }

        private int GetActionIndex(PlayerAction action)
        {
            return (int) action;
        }

        private PlayerAction GetAction(int index)
        {
            return (PlayerAction) index;
        }

        public int GetPressedActionCount()
        {
            return m_pressedCount;
        }

        public bool IsActive
        {
            get { return m_active; }
            set { m_active = value; }
        }
    }
}
