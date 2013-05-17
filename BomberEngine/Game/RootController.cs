using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual;
using BomberEngine.Debugging;
using Microsoft.Xna.Framework.Input;

namespace BomberEngine.Game
{
    public abstract class RootController : BaseElement
    {   
        protected Controller currentController;
        protected GameConsole console;

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

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
        
        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Updatable

        public override void Update(float delta)
        {
            currentController.Update(delta);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Drawable

        public override void Draw(Context context)
        {
            currentController.Draw(context);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Child controllers

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

                if (controller.ParentController == currentController)
                {
                    currentController.Suspend();
                }
                else
                {
                    currentController.Stop();
                }
                
            }

            currentController = controller;
            currentController.Start();
        }

        internal void ControllerStopped(Controller controller)
        {
            Debug.Assert(controller == currentController);
            currentController = null;

            OnControllerStop(controller);
        }

        protected virtual void OnControllerStop(Controller controller)
        {

        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Console

        protected abstract GameConsole CreateConsole();

        protected void ToggleConsole()
        {
            if (console != null)
            {
                if (currentController.IsCurrentScreen(console))
                {
                    console.Finish();
                }
                else
                {
                    currentController.StartNextScreen(console);
                }
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region InputListener

        public override bool OnKeyPressed(Keys key)
        {
            return currentController.OnKeyPressed(key);
        }

        public override bool OnKeyReleased(Keys key)
        {
            return currentController.OnKeyReleased(key);
        }

        public override bool OnButtonPressed(ButtonEvent e)
        {
            return currentController.OnButtonPressed(e);
        }

        public override bool OnButtonReleased(ButtonEvent e)
        {
            return currentController.OnButtonReleased(e);
        }

        public override void OnGamePadConnected(int playerIndex)
        {
            currentController.OnGamePadConnected(playerIndex);
        }

        public override void OnGamePadDisconnected(int playerIndex)
        {
            currentController.OnGamePadDisconnected(playerIndex);
        }

        public override void OnPointerMoved(int x, int y, int fingerId)
        {
            currentController.OnPointerMoved(x, y, fingerId);
        }

        public override void OnPointerPressed(int x, int y, int fingerId)
        {
            currentController.OnPointerPressed(x, y, fingerId);
        }

        public override void OnPointerDragged(int x, int y, int fingerId)
        {
            currentController.OnPointerDragged(x, y, fingerId);
        }

        public override void OnPointerReleased(int x, int y, int fingerId)
        {
            currentController.OnPointerReleased(x, y, fingerId);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public Controller CurrentController
        {
            get { return currentController; }
        }

        #endregion
    }
}