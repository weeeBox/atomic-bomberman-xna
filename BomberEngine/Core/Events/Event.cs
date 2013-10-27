
namespace BomberEngine
{
    public class Event
    {
        public const int KEY = 1;
        public const int TOUCH = 2;
        public const int GAMEPAD = 3;

        public int code;

        public Event(int code)
        {
            this.code = code;
        }
    }
}
