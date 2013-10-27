
namespace BomberEngine
{
    public enum MouseState
    {
        Pressed,
        Dragged,
        Released
    }

    public class MouseEvent : Event
    {
        public int x;
        public int y;
        public MouseState state;

        public MouseEvent()
            : base(Event.TOUCH)
        {
        }
    }
}
