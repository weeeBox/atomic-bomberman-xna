using Microsoft.Xna.Framework.Input;

namespace BomberEngine.Core.Input
{
    public struct ButtonEventArg
    {
        public Buttons button;
        public int playerIndex;

        public ButtonEventArg(int playerIndex, Buttons button)
        {
            this.playerIndex = playerIndex;
            this.button = button;
        }
    }

    public interface IGamePadListener
    {
        bool OnButtonPressed(ButtonEventArg e);
        bool OnButtonRepeat(ButtonEventArg e);
        bool OnButtonReleased(ButtonEventArg e);
    }
}
