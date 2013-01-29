using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;

namespace Bomberman.Game.Elements.Players.Input
{
    public class PlayerGamePadInput : PlayerInput, GamePadListener
    {
        private Dictionary<Buttons, PlayerAction> actionLookup;
        private int playerIndex;

        public PlayerGamePadInput(int playerIndex)
        {
            this.playerIndex = playerIndex;
            actionLookup = new Dictionary<Buttons, PlayerAction>();
        }

        public void Map(Buttons button, PlayerAction action)
        {
            actionLookup.Add(button, action);
        }

        public void ButtonPressed(ButtonEvent e)
        {
            if (e.playerIndex == playerIndex)
            {
                PlayerAction action = GetAction(e.button);
                if (action != PlayerAction.Count)
                {
                    NotifyActionPressed(action);
                }
            }
        }

        public void ButtonReleased(ButtonEvent e)
        {
            if (e.playerIndex == playerIndex)
            {
                PlayerAction action = GetAction(e.button);
                if (action != PlayerAction.Count)
                {
                    NotifyActionReleased(action);
                }
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
