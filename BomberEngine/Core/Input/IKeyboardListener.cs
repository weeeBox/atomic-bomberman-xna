using Microsoft.Xna.Framework.Input;

namespace BomberEngine.Core.Input
{   
    public interface IKeyboardListener
    {
        bool OnKeyPressed(Keys key);
        bool OnKeyReleased(Keys key);
    }
}
