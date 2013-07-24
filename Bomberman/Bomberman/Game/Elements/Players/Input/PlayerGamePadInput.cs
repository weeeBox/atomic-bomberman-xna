using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Input;
using BomberEngine.Core;
using BomberEngine.Game;
using Microsoft.Xna.Framework;
using BomberEngine.Util;
using BomberEngine.Core.Events;

namespace Bomberman.Game.Elements.Players.Input
{
    public class PlayerGamePadInput : PlayerKeyInput
    {
        private const float STICK_DEAD_ZONE = 0.125f;
        private const float STICK_DEAD_ZONE_2 = STICK_DEAD_ZONE * STICK_DEAD_ZONE;

        private const float STICK_LIMIT_MIN = 0.75f;
        private const float STICK_LIMIT_MAX = 0.75f;

        private int playerIndex;

        private float m_dx;
        private float m_dy;

        public PlayerGamePadInput(int playerIndex)
        {
            this.playerIndex = playerIndex;
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            Vector2 stick = Application.Input().ThumbSticks(playerIndex).Left;
            float dx = stick.X;
            float dy = stick.Y;

            if (dx != m_dx || dy != m_dy)
            {
                bool up = false;
                bool down = false;
                bool left = false;
                bool right = false;

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

                SetActionPressed(PlayerAction.Up, up);
                SetActionPressed(PlayerAction.Down, down);
                SetActionPressed(PlayerAction.Left, left);
                SetActionPressed(PlayerAction.Right, right);

                m_dx = dx;
                m_dy = dy;
            }
        }
    }
}