namespace BomberEngine.Core.Input
{
    public interface ITouchListener
    {
        void PointerMoved(int x, int y, int fingerId);
        void PointerPressed(int x, int y, int fingerId);
        void PointerDragged(int x, int y, int fingerId);
        void PointerReleased(int x, int y, int fingerId);
    }
}
