using System.Collections.Generic;
using BomberEngine;

namespace Bomberman.Gameplay.Elements.Players
{
    public class PlayerKeyInput : PlayerBitArrayInput
    {
        public override bool IsLocal
        {
            get { return true; }
        }
    }
}
