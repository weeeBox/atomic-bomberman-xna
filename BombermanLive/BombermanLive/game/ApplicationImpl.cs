using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using core;
using Microsoft.Xna.Framework;

namespace game
{
    public class ApplicationImpl : Application
    {
        bool running;

        public void Start()
        {
            running = true;
        }

        public void Update(float delta)
        {
            
        }

        public void Stop()
        {
            
        }

        public bool isRunning()
        {
            return running;
        }

        public void Draw(GraphicsDeviceManager graphics)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        }
    }
}
