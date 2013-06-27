using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine;
using BomberEngine.Debugging;
using BomberEngine.Core;

namespace Bomberman.Game.Elements.Players.Input
{
    public abstract class PlayerInput : IUpdatable
    {
        private int stateBits;
        private int stateBitsOld;

        private int pressedCount;

        public virtual void Update(float delta)
        {   
        }

        public void SaveState()
        {
            stateBitsOld = stateBits;
        }

        protected void NotifyActionPressed(PlayerAction action)
        {
            int index = GetIndex(action);

            Debug.Assert(!IsActionPressed(index), "Action already pressed: " + action);
            SetActionPressed(index);
            ++pressedCount;
        }

        protected void NotifyActionReleased(PlayerAction action)
        {
            int index = GetIndex(action);

            Debug.Assert(IsActionPressed(index), "Action not pressed: " + action);
            Debug.Assert(pressedCount > 0, "Invalid pressed counter: " + pressedCount);
            SetActionReleased(index);
            --pressedCount;
        }

        protected void ReleaseAllActions()
        {
            stateBits = 0;
        }

        public bool IsActionPressed(PlayerAction action)
        {
            int index = GetIndex(action);
            return IsActionPressed(index);
        }

        public bool IsActionJustPressed(PlayerAction action)
        {
            int index = GetIndex(action);
            return IsActionJustPressed(index);
        }

        public bool IsActionJustReleased(PlayerAction action)
        {
            int index = GetIndex(action);
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

        private void SetActionPressed(int index)
        {
            stateBits |= 1 << index;
        }

        private void SetActionReleased(int index)
        {
            stateBits &= ~(1 << index);
        }

        public int GetPressedActionCount()
        {
            return pressedCount;
        }

        private int GetIndex(PlayerAction action)
        {
            return (int) action;
        }

        private PlayerAction GetAction(int index)
        {
            return (PlayerAction) index;
        }
    }
}
