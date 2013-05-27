using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;

namespace Bomberman.Game.Elements.Players.Input
{
    public class PlayerKeyboardInput : PlayerInput, IKeyInputListener
    {
        private Dictionary<KeyCode, PlayerAction> actionLookup;

        public PlayerKeyboardInput()
        {
            actionLookup = new Dictionary<KeyCode, PlayerAction>();
        }

        public void Map(KeyCode key, PlayerAction action)
        {
            actionLookup.Add(key, action);
        }

        public bool OnKeyPressed(KeyEventArg e)
        {
            PlayerAction action = GetAction(e.key);
            if (action != PlayerAction.Count)
            {
                NotifyActionPressed(action);
                return true;
            }

            return false;
        }

        public bool OnKeyRepeated(KeyEventArg e)
        {
            return false; // TODO: handle key repeat
        }

        public bool OnKeyReleased(KeyEventArg e)
        {
            PlayerAction action = GetAction(e.key);
            if (action != PlayerAction.Count)
            {
                NotifyActionReleased(action);
                return true;
            }

            return false;
        }

        private PlayerAction GetAction(KeyCode key)
        {
            if (actionLookup.ContainsKey(key))
            {
                return actionLookup[key];
            }
            return PlayerAction.Count;
        }
    }
}
