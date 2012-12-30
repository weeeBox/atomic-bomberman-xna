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
        private IEnumerable<Updatable> updatables;
        private IEnumerable<Drawable> drawables;

        private Context context;

        private bool running;

        public ApplicationImpl(GraphicsDeviceManager graphics)
        {
            context = new ContextImpl(graphics);
            updatables = new LinkedList<Updatable>();
            drawables = new LinkedList<Drawable>();
        }

        public void Start()
        {
            running = true;
        }

        public void Update(float delta)
        {
            foreach (Updatable e in updatables)
            {
                e.update(delta);
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

        public void Draw()
        {
            foreach (Drawable e in drawables)
            {
                e.Draw(context);
            }
        }
    }
}
