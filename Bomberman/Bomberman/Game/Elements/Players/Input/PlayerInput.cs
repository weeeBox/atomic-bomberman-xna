using BomberEngine;

namespace Bomberman.Gameplay.Elements.Players
{
    public abstract class PlayerInput : IUpdatable, IResettable
    {
        private int m_stateBits;
        private int m_stateBitsOld;

        public PlayerInput()
        {   
        }

        public virtual void Update(float delta)
        {
            m_stateBitsOld = m_stateBits;
        }

        public virtual void Reset()
        {
            m_stateBits = 0;
            m_stateBitsOld = 0;
        }

        public void Force(int mask)
        {
            m_stateBits = m_stateBitsOld = mask;
        }

        public void SetActionPressed(PlayerAction action, bool flag)
        {
            int index = GetActionIndex(action);
            SetActionPressed(index, flag);
        }

        public void SetActionPressed(int index, bool flag)
        {   
            if (flag)
            {   
                m_stateBits |= 1 << index;
            }
            else
            {
                m_stateBits &= ~(1 << index);
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
            int pressedCount = 0;
            int actionsCount = (int)PlayerAction.Count;
            for (int i = 0; i < actionsCount; ++i)
            {
                if (IsActionPressed(i))
                {
                    ++pressedCount;
                }
            }

            return pressedCount;
        }

        public int mask
        {
            get { return m_stateBits; }
        }

        public abstract bool IsLocal
        {
            get;
        }
    }
}
