using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine;
using BomberEngine.Debugging;

namespace Bomberman.Game.Elements.Players.Input
{
    public abstract class PlayerInput
    {
        public Player player;

        private bool[] actionsPressedState;
        private int pressedCount;

        public PlayerInput()
        {   
            actionsPressedState = new bool[GetIndex(PlayerAction.Count)];
        }

        protected void NotifyActionPressed(PlayerAction action)
        {
            int index = GetIndex(action);

            Debug.Assert(!actionsPressedState[index], "Action already pressed: " + action);
            actionsPressedState[index] = true;
            ++pressedCount;

            if (player != null)
            {
                player.OnActionPressed(this, action);
            }
        }

        protected void NotifyActionReleased(PlayerAction action)
        {
            int index = GetIndex(action);

            Debug.Assert(actionsPressedState[index], "Action not pressed: " + action);
            Debug.Assert(pressedCount > 0, "Invalid pressed counter: " + pressedCount);
            actionsPressedState[index] = false;
            --pressedCount;

            if (player != null)
            {
                player.OnActionReleased(this, action);
            }
        }

        protected void ReleaseAllActions()
        {
            if (pressedCount > 0)
            {
                for (int i = 0; i < actionsPressedState.Length; ++i)
                {
                    if (actionsPressedState[i])
                    {
                        Debug.Assert(pressedCount > 0, "Invalid pressed counter: " + pressedCount);
                        --pressedCount;

                        if (player != null)
                        {
                            player.OnActionReleased(this, GetAction(i));
                        }
                        actionsPressedState[i] = false;
                    }
                }
                Debug.Assert(pressedCount == 0, "Invalid pressed counter: " + pressedCount);
            }
        }

        public bool IsActionPressed(PlayerAction action)
        {
            int index = GetIndex(action);
            return actionsPressedState[index];
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
