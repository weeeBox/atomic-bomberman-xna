
namespace BomberEngine
{
    public interface IKeyInputListener
    {
        bool OnKeyPressed(KeyEventArg arg);
        bool OnKeyRepeated(KeyEventArg arg);
        bool OnKeyReleased(KeyEventArg arg);
    }
}
