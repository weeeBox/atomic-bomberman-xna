using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Game.Elements.Players.Input
{
    public interface PlayerInputListener
    {
        void OnActionPressed(PlayerInput playerInput, PlayerAction action);
        void OnActionReleased(PlayerInput playerInput, PlayerAction action);
    }
}