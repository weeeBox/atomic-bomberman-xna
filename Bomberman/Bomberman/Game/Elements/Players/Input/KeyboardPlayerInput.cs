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
        private Dictionary<Keys, PlayerAction> keysMap;

        public KeyboardPlayerInput()
        {
            keysMap = new Dictionary<Keys, PlayerAction>();
        }

        public void Map(Keys key, PlayerAction action)
        {
            keysMap.Add(key, action);
        }

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
            if (keysMap.ContainsKey(key))
            {
                return keysMap[key];
            }
            return PlayerAction.Count;
        }
    }
}
