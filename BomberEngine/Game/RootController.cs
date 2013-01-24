using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual;

namespace BomberEngine.Game
{
    public class RootController : Updatable, Drawable
    {   
        private Controller currentController;

        public RootController()
        {   
        }

        public void Start()
        {
            OnStart();
        }

        public void Stop()
        {
            OnStop();
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnStop()
        {
        }

        public void Update(float delta)
        {
            currentController.Update(delta);
        }

        public void Draw(Context context)
        {
            currentController.Draw(context);
        }

        public void StartController(Controller controller)
        {
            if (controller == null)
            {
                throw new ArgumentException("Controller is null");
            }

            if (currentController != null)
            {
                if (controller == currentController)
                {
                    throw new InvalidOperationException("Controller already set as current: " + controller);
                }

                currentController.Stop();
            }

            currentController = controller;
            controller.Start();
        }
    }
}