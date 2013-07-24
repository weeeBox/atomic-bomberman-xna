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
        private int stateBits;
        private int stateBitsOld;

        private int pressedCount;

        public virtual void Update(float delta)
        {
            stateBitsOld = stateBits;
        }

        public void Reset()
        {
            stateBits = 0;
            stateBitsOld = 0;
            pressedCount = 0;
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
                stateBits |= 1 << index;
                if (IsActionJustPressed(index))
                {
                    ++pressedCount;
                }
            }
            else
            {
                stateBits &= ~(1 << index);
                if (IsActionJustReleased(index))
                {
                    Debug.Assert(pressedCount > 0);
                    --pressedCount;
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
            return IsActionPressed(stateBits, index);
        }

        public bool IsActionJustPressed(int index)
        {
            return IsActionPressed(stateBits, index) && !IsActionPressed(stateBitsOld, index);
        }

        public bool IsActionJustReleased(int index)
        {
            return !IsActionPressed(stateBits, index) && IsActionPressed(stateBitsOld, index);
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
            return pressedCount;
        }
    }
}
