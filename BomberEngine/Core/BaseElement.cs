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

        public virtual bool OnKeyPressed(Keys key)
        {
            return false;
        }

        public virtual bool OnKeyReleased(Keys key)
        {
            return false;
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

        public virtual void OnPointerMoved(int x, int y, int fingerId)
        {
        }

        public virtual void OnPointerPressed(int x, int y, int fingerId)
        {
        }

        public virtual void OnPointerDragged(int x, int y, int fingerId)
        {
        }

        public virtual void OnPointerReleased(int x, int y, int fingerId)
        {
        }
    }
}
