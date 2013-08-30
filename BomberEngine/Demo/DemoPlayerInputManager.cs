using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework;

namespace BomberEngine.Demo
{
    public class DemoPlayerInputManager : InputManager
    {
        public override bool IsKeyPressed(KeyCode code)
        {
            throw new NotImplementedException();
        }

        public override bool IsButtonPressed(int playerIndex, KeyCode code)
        {
            throw new NotImplementedException();
        }

        public override bool IsGamePadConnected(int playerIndex)
        {
            throw new NotImplementedException();
        }

        public override Vector2 LeftThumbStick(int playerIndex = 0)
        {
            throw new NotImplementedException();
        }

        public override Vector2 RightThumbStick(int playerIndex = 0)
        {
            throw new NotImplementedException();
        }

        public override float LeftTrigger(int playerIndex = 0)
        {
            throw new NotImplementedException();
        }

        public override float RightTrigger(int playerIndex = 0)
        {
            throw new NotImplementedException();
        }
    }
}
