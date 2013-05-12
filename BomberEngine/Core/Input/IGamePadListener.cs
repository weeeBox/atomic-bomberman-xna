﻿using Microsoft.Xna.Framework.Input;

namespace BomberEngine.Core.Input
{
    public struct ButtonEvent
    {
        public Buttons button;
        public int playerIndex;

        public ButtonEvent(int playerIndex, Buttons button)
        {
            this.playerIndex = playerIndex;
            this.button = button;
        }
    }

    public interface IGamePadListener
    {
        void OnButtonPressed(ButtonEvent e);
        void OnButtonReleased(ButtonEvent e);
    }
}
