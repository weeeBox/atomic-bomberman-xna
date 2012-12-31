using Microsoft.Xna.Framework;

namespace core
{
    public interface Application
    {
        void Start();
        void Stop();

        void Update(float delta);
        void Draw();
        
        bool IsRunning();
    }
}
