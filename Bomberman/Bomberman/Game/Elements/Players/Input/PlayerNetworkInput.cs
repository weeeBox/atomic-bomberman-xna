using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;

namespace Bomberman.Game.Elements.Players.Input
{
    public class PlayerNetworkInput : PlayerInput
    {
        public void SetActionFlag(int index, bool flag)
        {
            Debug.Assert(index >= 0 && index < (int)PlayerAction.Count);
            SetActionPressed(index, flag);
        }
    }
}
