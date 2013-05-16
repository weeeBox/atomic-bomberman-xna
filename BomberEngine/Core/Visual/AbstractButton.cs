using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core.Input;

namespace BomberEngine.Core.Visual
{
    public interface IButtonDelegate
    {
        void OnButtonPress(AbstractButton button);
    }

    public class AbstractButton : View
    {
        protected IButtonDelegate buttonDelegate;

        public AbstractButton(int width, int height)
            : this(0, 0, width, height)
        {
        }

        public AbstractButton(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            focusable = true;
        }

        public override bool OnKeyPressed(Keys key)
        {
            if (key == Keys.Enter)
            {
                OnPress();
                return true;
            }

            return base.OnKeyPressed(key);
        }

        public override bool OnKeyReleased(Keys key)
        {
            if (key == Keys.Enter)
            {
                OnRelease();
                return true;
            }

            return base.OnKeyReleased(key);
        }

        public override bool OnButtonPressed(ButtonEvent e)
        {   
            if (e.button == Buttons.A)
            {
                OnPress();
                return true;
            }
            
            return base.OnButtonPressed(e);
        }

        public override bool OnButtonReleased(ButtonEvent e)
        {
            if (e.button == Buttons.A)
            {
                OnRelease();
                return true;
            }

            return base.OnButtonReleased(e);
        }

        protected virtual void OnPress()
        {
            if (buttonDelegate != null)
            {
                buttonDelegate.OnButtonPress(this);
            }
        }

        protected virtual void OnRelease()
        {
        }
    }
}
