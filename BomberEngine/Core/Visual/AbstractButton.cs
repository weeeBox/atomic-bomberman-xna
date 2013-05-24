using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core.Input;
using BomberEngine.Core.Events;

namespace BomberEngine.Core.Visual
{
    public interface IButtonDelegate
    {
        void OnButtonPress(AbstractButton button);
    }

    public class AbstractButton : View
    {
        protected IButtonDelegate buttonDelegate;

        public AbstractButton(float width, float height)
            : this(0, 0, width, height)
        {
        }

        public AbstractButton(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
            focusable = true;
        }

        public override bool HandleEvent(Event evt)
        {
            if (evt.code == Event.GAMEPAD)
            {
                GamePadEvent gamepadEvt = evt as GamePadEvent;
                if (gamepadEvt.arg.button == Buttons.A)
                {
                    if (gamepadEvt.state == GamePadEvent.PRESSED)
                    {
                        OnPress();
                        return true;
                    }

                    if (gamepadEvt.state == GamePadEvent.RELEASED)
                    {
                        OnRelease();
                        return true;
                    }
                }
            }
            else if (evt.code == Event.KEYBOARD)
            {
                KeyboardEvent keyEvent = evt as KeyboardEvent;
                if (keyEvent.key == Keys.Enter)
                {
                    if (keyEvent.state == KeyboardEvent.PRESSED)
                    {
                        OnPress();
                        return true;                        
                    }

                    if (keyEvent.state == KeyboardEvent.RELEASED)
                    {
                        OnRelease();
                        return true;
                    }
                }
            }

            return base.HandleEvent(evt);
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

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public IButtonDelegate Delegate()
        {
            return buttonDelegate;
        }

        public void SetDelegate(IButtonDelegate buttonDelegate)
        {
            this.buttonDelegate = buttonDelegate;
        }

        #endregion
    }
}
