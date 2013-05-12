using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;

namespace Bomberman.Game.Elements.Players.Input
{
    public class PlayerKeyboardInput : PlayerInput, IKeyboardListener
    {
        private Dictionary<Keys, PlayerAction> actionLookup;

        public PlayerKeyboardInput()
        {
            actionLookup = new Dictionary<Keys, PlayerAction>();
        }

        public void Map(Keys key, PlayerAction action)
        {
            actionLookup.Add(key, action);
        }

        public void OnKeyPressed(Keys key)
        {
            PlayerAction action = GetAction(key);
            if (action != PlayerAction.Count)
            {
                NotifyActionPressed(action);
            }
        }

        public void OnKeyReleased(Keys key)
        {
            PlayerAction action = GetAction(key);
            if (action != PlayerAction.Count)
            {
                NotifyActionReleased(action);
            }
        }

        private PlayerAction GetAction(Keys key)
        {
            if (actionLookup.ContainsKey(key))
            {
                return actionLookup[key];
            }
            return PlayerAction.Count;
        }
    }
}
