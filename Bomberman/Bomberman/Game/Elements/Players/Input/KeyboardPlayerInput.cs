using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;

namespace Bomberman.Game.Elements.Players.Input
{
    public class KeyboardPlayerInput : PlayerInput, KeyboardListener
    {
        public void KeyPressed(Keys key)
        {
            PlayerAction action = GetAction(key);
            if (action != PlayerAction.Count)
            {
                NotifyActionPressed(action);
            }
        }

        public void KeyReleased(Keys key)
        {
            PlayerAction action = GetAction(key);
            if (action != PlayerAction.Count)
            {
                NotifyActionReleased(action);
            }
        }

        private PlayerAction GetAction(Keys key)
        {
            switch (key)
            {
                case Keys.W:
                    return PlayerAction.Up;

                case Keys.S:
                    return PlayerAction.Down;

                case Keys.A:
                    return PlayerAction.Left;

                case Keys.D:
                    return PlayerAction.Right;
            }

            return PlayerAction.Count;
        }
    }
}
