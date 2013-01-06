using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using Microsoft.Xna.Framework;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;

namespace BomberEngine.Game
{
    public abstract class Application
    {   
        private RootController rootController;

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
            rootController.Start();

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
            rootController.Stop();

            OnStop();
        }

        protected virtual void OnStart()
        {

        }

        protected virtual void OnStop()
        {

        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public void Update(float delta)
        {   
            rootController.Update(delta);
        }

        public void Draw()
        {
            rootController.Draw(context);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public bool IsRunning
        {
            get { return !stoped; }
        }

        public RootController RootController
        {
            get { return rootController; }
            set { rootController = value; }
        }

        public InputManager InputManager
        {
            get { return InputManager; }
        }

        #endregion
    }
}
