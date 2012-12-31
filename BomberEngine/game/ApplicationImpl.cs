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
        private Updatable updatable;
        private Drawable drawable;

        private Context context;

        private bool running;

        public ApplicationImpl(GraphicsDeviceManager graphics)
        {
            context = new ContextImpl(graphics);

            Global.application = this;
            Global.timerManager = new TimerManager();
       }

        public void Start()
        {
            running = true;
        }

        public void Update(float delta)
        {
            if (updatable != null)
            {
                updatable.Update(delta);
            }
        }

        public void Draw()
        {
            if (drawable != null)
            {
                drawable.Draw(context);
            }
        }

        public void Stop()
        {
            running = false;
        }

        public bool IsRunning()
        {
            return running;
        }
    }
}
