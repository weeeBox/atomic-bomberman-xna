using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Game.Elements.Players.Input
{
    public class PlayerNetworkInput : PlayerInput
    {
        public bool OnActionPressed(PlayerAction action)
        {   
            if (!IsActionPressed(action))
            {
                NotifyActionPressed(action);
                return true;
            }

            return false;
        }

        public bool OnActionReleased(PlayerAction action)
        {
            if (IsActionPressed(action))
            {
                NotifyActionReleased(action);
                return true;
            }

            return false;
        }
    }
}
