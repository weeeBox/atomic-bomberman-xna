using Microsoft.Xna.Framework.Input;

namespace BomberEngine.Core.Input
{   
    public interface IKeyboardListener
    {
        void OnKeyPressed(Keys key);
        void OnKeyReleased(Keys key);
    }
}
