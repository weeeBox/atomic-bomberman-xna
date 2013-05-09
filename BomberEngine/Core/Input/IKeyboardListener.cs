using Microsoft.Xna.Framework.Input;

namespace BomberEngine.Core.Input
{   
    public interface IKeyboardListener
    {
        void KeyPressed(Keys key);
        void KeyReleased(Keys key);
    }
}
