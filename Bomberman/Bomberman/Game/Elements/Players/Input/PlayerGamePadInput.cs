using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core;
using BomberEngine.Game;
using Microsoft.Xna.Framework;
using BomberEngine.Util;

namespace Bomberman.Game.Elements.Players.Input
{
    public class PlayerGamePadInput : PlayerInput, IGamePadListener, IUpdatable
    {
        private const float STICK_DEAD_ZONE = 0.125f;
        private const float STICK_DEAD_ZONE_2 = STICK_DEAD_ZONE * STICK_DEAD_ZONE;

        private const float STICK_LIMIT_MIN = 0.75f;
        private const float STICK_LIMIT_MAX = 0.75f;

        private Dictionary<Buttons, PlayerAction> actionLookup;
        private int playerIndex;

        private float m_dx;
        private float m_dy;

        private bool up;
        private bool down;
        private bool left;
        private bool right;

        public PlayerGamePadInput(int playerIndex)
        {
            this.playerIndex = playerIndex;
            actionLookup = new Dictionary<Buttons, PlayerAction>();
        }

        public void Update(float delta)
        {
            Vector2 stick = Application.Input().ThumbSticks(playerIndex).Left;
            float dx = stick.X;
            float dy = stick.Y;

            if (dx != m_dx || dy != m_dy)
            {
                bool oldUp = up;
                bool oldDown = down;
                bool oldLeft = left;
                bool oldRight = right;

                up = down = left = right = false;

                float len2 = dx * dx + dy * dy;
                if (len2 > STICK_DEAD_ZONE_2)
                {   
                    float lenOver1 = (float)(1.0 / Math.Sqrt(len2));
                    float ndx = dx * lenOver1;
                    float ndy = dy * lenOver1;

                    float adx = Math.Abs(ndx);
                    float ady = Math.Abs(ndy);

                    float limit = !up && !down ? STICK_LIMIT_MAX : STICK_LIMIT_MIN;
                    left = dx < 0 && ady <= limit;
                    right = dx > 0 && ady <= limit;

                    limit = !left && !right ? STICK_LIMIT_MAX : STICK_LIMIT_MIN;
                    up = dy > 0 && adx <= limit;
                    down = dy < 0 && adx <= limit; 
                }

                TryNofityAction(oldUp, up, PlayerAction.Up);
                TryNofityAction(oldDown, down, PlayerAction.Down);
                TryNofityAction(oldLeft, left, PlayerAction.Left);
                TryNofityAction(oldRight, right, PlayerAction.Right);

                m_dx = dx;
                m_dy = dy;
            }
        }

        public void Map(Buttons button, PlayerAction action)
        {
            actionLookup.Add(button, action);
        }

        public bool OnButtonPressed(ButtonEventArg e)
        {
            if (e.playerIndex == playerIndex)
            {
                PlayerAction action = GetAction(e.button);
                if (action != PlayerAction.Count)
                {
                    NotifyActionPressed(action);
                    return true;
                }
            }
            return false;
        }

        public bool OnButtonRepeat(ButtonEventArg e)
        {
            return false;
        }

        public bool OnButtonReleased(ButtonEventArg e)
        {
            if (e.playerIndex == playerIndex)
            {
                PlayerAction action = GetAction(e.button);
                if (action != PlayerAction.Count)
                {
                    NotifyActionReleased(action);
                    return true;
                }
            }
            return false;
        }

        private void TryNofityAction(bool oldFlag, bool flag, PlayerAction action)
        {
            if (!oldFlag && flag)
            {
                NotifyActionPressed(action);
            }
            else if (oldFlag && !flag)
            {
                NotifyActionReleased(action);
            }
        }

        private PlayerAction GetAction(Buttons button)
        {
            if (actionLookup.ContainsKey(button))
            {
                return actionLookup[button];
            }
            return PlayerAction.Count;
        }
    }
}
