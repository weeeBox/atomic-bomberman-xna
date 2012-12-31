using Microsoft.Xna.Framework.Input;

namespace BomberEngine.Core.Input
{   
    public interface KeyboardListener
    {
        void KeyPressed(Keys key);
        void KeyReleased(Keys key);
    }
}
