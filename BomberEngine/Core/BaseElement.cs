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

        public virtual void OnKeyPressed(Keys key)
        {   
        }

        public virtual void OnKeyReleased(Keys key)
        {   
        }

        public virtual void OnButtonPressed(ButtonEvent e)
        {
        }

        public virtual void OnButtonReleased(ButtonEvent e)
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
