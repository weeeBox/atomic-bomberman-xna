using Microsoft.Xna.Framework;

namespace core
{
    public interface Application
    {
        void Start();
        void Update(float delta);
        void Stop();

        bool isRunning();

        void Draw(GraphicsDeviceManager graphics);
    }
}
