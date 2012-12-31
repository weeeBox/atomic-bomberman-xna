using Microsoft.Xna.Framework;

namespace BomberEngine.Core
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
