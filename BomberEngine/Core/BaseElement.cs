using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core.Visual;

namespace BomberEngine.Core
{
    public abstract class BaseElement : IUpdatable, IDrawable, IInputListener
    {
        public virtual void Update(float delta)
        {   
        }

        public virtual void Draw(Context context)
        {
        }

        public virtual void KeyPressed(Keys key)
        {   
        }

        public virtual void KeyReleased(Keys key)
        {   
        }

        public virtual void ButtonPressed(ButtonEvent e)
        {
        }

        public virtual void ButtonReleased(ButtonEvent e)
        {
        }

        public virtual void GamePadConnected(int playerIndex)
        {
        }

        public virtual void GamePadDisconnected(int playerIndex)
        {
        }

        public virtual void PointerMoved(int x, int y, int fingerId)
        {
        }

        public virtual void PointerPressed(int x, int y, int fingerId)
        {
        }

        public virtual void PointerDragged(int x, int y, int fingerId)
        {
        }

        public virtual void PointerReleased(int x, int y, int fingerId)
        {
        }
    }
}
