using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using Microsoft.Xna.Framework;

namespace BomberEngine.Game
{
    public abstract class Application
    {
        private Updatable updatable;
        private Drawable drawable;

        private Context context;

        private bool started;
        private bool stoped;

        public Application(GraphicsDeviceManager graphics)
        {
            context = new ContextImpl(graphics);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        public void Start()
        {
            if (started)
            {
                throw new InvalidOperationException("Application already started");
            }
            started = true;
            OnStart();
        }

        public void Stop()
        {
            if (!started)
            {
                throw new InvalidOperationException("Application not started");
            }

            if (stoped)
            {
                throw new InvalidOperationException("Application already stopped");
            }

            stoped = true;
            OnStop();
        }

        protected void OnStart()
        {

        }

        protected void OnStop()
        {

        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public void Update(float delta)
        {
            updatable.Update(delta);
        }

        public void Draw()
        {
            drawable.Draw(context);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public bool IsRunning
        {
            get { return !stoped; }
        }

        protected Updatable Updatable
        {
            get { return updatable; }
            set { updatable = value; }
        }

        protected Drawable Drawable
        {
            get { return drawable; }
            set { drawable = value; }
        }

        #endregion
    }
}
