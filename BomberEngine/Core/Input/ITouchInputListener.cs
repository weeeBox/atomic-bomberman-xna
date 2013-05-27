namespace BomberEngine.Core.Input
{
    public interface ITouchInputListener
    {
        void OnPointerMoved(int x, int y, int fingerId);
        void OnPointerPressed(int x, int y, int fingerId);
        void OnPointerDragged(int x, int y, int fingerId);
        void OnPointerReleased(int x, int y, int fingerId);
    }
}
