using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual;
using BomberEngine.Debugging;

namespace BomberEngine.Game
{
    public abstract class RootController : GameObject
    {   
        private Controller currentController;
        private GameConsole console;

        public RootController()
        {   
        }

        public void Start()
        {
            console = CreateConsole();
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

        public override void Update(float delta)
        {
            currentController.Update(delta);
        }

        public override void Draw(Context context)
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

            Application.Input().SetInputListener(controller);
            controller.Start();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Console

        protected abstract GameConsole CreateConsole();

        protected void ToggleConsole()
        {
            if (console != null)
            {
                if (currentController.IsCurrentScene(console))
                {
                    console.Finish();
                }
                else
                {
                    currentController.StartNextScene(console);
                }
            }
        }

        #endregion
    }
}