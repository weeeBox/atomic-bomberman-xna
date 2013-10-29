using System;

namespace BomberEngine
{
    public delegate void ButtonDelegate(Button button);
    
    public class Button : View
    {
        public ButtonDelegate buttonDelegate;
        public Object data;

        public Button()
            : this(0, 0, 0, 0)
        {
        }

        public Button(float width, float height)
            : this(0, 0, width, height)
        {
        }

        public Button(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
            focusable = true;
        }

        public override bool HandleEvent(Event evt)
        {
            if (evt.code == Event.KEY)
            {
                KeyEvent keyEvent = evt as KeyEvent;
                if (keyEvent.IsConfirmKey())
                {
                    if (keyEvent.IsPressed)
                    {
                        OnPress();
                        return true;                        
                    }

                    if (keyEvent.IsReleased)
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
            Application.ScheduleTimer(NotifyDelegate);
        }

        protected virtual void OnRelease()
        {
        }

        protected void NotifyDelegate()
        {
            if (buttonDelegate != null)
            {
                buttonDelegate(this);
            }
        }
    }
}
