using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Input;
using BomberEngine.Core.Events;

namespace Bomberman.Game.Elements.Players.Input
{
    public class PlayerKeyboardInput : PlayerInput, IEventHandler
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

        public bool HandleEvent(Event evt)
        {
            if (evt.code == Event.KEY)
            {
                KeyEvent keyEvent = evt as KeyEvent;
                if (keyEvent.state == KeyState.Pressed)
                {
                    return OnKeyPressed(keyEvent.arg);
                }

                if (keyEvent.state == KeyState.Released)
                {
                    return OnKeyReleased(keyEvent.arg);
                }
            }

            return false;
        }

        private bool OnKeyPressed(KeyEventArg e)
        {
            PlayerAction action = GetAction(e.key);
            if (action != PlayerAction.Count)
            {
                NotifyActionPressed(action);
                return true;
            }

            return false;
        }

        private bool OnKeyReleased(KeyEventArg e)
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
