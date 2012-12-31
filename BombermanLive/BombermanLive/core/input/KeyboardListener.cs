using Microsoft.Xna.Framework.Input;

namespace core.input
{   
    public interface KeyboardListener
    {
        void KeyPressed(Keys key);
        void KeyReleased(Keys key);
    }
}
