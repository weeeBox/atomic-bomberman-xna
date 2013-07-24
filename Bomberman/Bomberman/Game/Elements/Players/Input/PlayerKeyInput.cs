using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Input;
using BomberEngine.Core.Events;
using BomberEngine.Game;

namespace Bomberman.Game.Elements.Players.Input
{
    public class PlayerKeyInput : PlayerInput
    {
        private Dictionary<KeyCode, PlayerAction> actionLookup;

        public PlayerKeyInput()
        {
            actionLookup = new Dictionary<KeyCode, PlayerAction>();
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            foreach (KeyValuePair<KeyCode, PlayerAction> e in actionLookup)
            {
                KeyCode key = e.Key;
                PlayerAction action = e.Value;
                SetActionPressed(action, Application.Input().IsKeyPressed(key));
            }
        }

        public void Map(KeyCode key, PlayerAction action)
        {
            actionLookup.Add(key, action);
        }
    }
}
