using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Debugging;
using BomberEngine.Core.Events;
using BomberEngine.Consoles;

namespace BomberEngine.Game
{
    public class Controller : BaseElement
    {
        protected ScreenManager screenManager;

        protected Controller childController;
        protected Controller parentController;

        public int exitCode;

        public Controller()
        {   
            screenManager = new ScreenManager();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Updatable

        public override void Update(float delta)
        {
            screenManager.Update(delta);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Drawable

        public override void Draw(Context context)
        {
            screenManager.Draw(context);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        public void Start()
        {   
            OnStart();
        }

        public void Stop()
        {
            Stop(exitCode);
        }

        public void Stop(int exitCode)
        {
            this.exitCode = exitCode;

            if (childController != null)
            {
                childController.parentController = null;
                childController.Stop();
            }

            if (parentController != null)
            {
                parentController.OnChildControllerStopped(this);
            }

            OnStop();
            Application.RootController().ControllerStopped(this);
        }

        public void Suspend()
        {
            OnSuspend();
        }

        public void Resume()
        {
            OnResume();
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnStop()
        {
        }

        protected virtual void OnSuspend()
        {
        }

        protected virtual void OnResume()
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        protected virtual void StartChildController(Controller controller)
        {
            if (controller == null)
            {
                throw new ArgumentException("Controller is null");
            }

            if (controller == childController)
            {
                throw new InvalidOperationException("Controller is already started");
            }

            if (childController != null)
            {
                throw new InvalidOperationException("Another child controller is already started");
            }

            childController = controller;
            childController.parentController = this;
            StartController(controller);
        }

        public virtual void StartController(Controller controller)
        {
            Application.RootController().StartController(controller);
        }

        private void StopChildController(Controller controller)
        {
            Debug.Assert(controller == childController);
            OnChildControllerStopped(controller);
            childController.parentController = null;
            childController = null;
        }

        protected virtual void OnChildControllerStopped(Controller controller)
        {
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Screens

        public Screen CurrentScreen()
        {
            return screenManager.CurrentScreen();
        }

        public bool IsCurrentScreen(Screen screen)
        {
            return screenManager.IsCurrent(screen);
        }

        public void StartScreen(Screen screen)
        {   
            screenManager.StartScreen(screen, true);
        }

        public void StartNextScreen(Screen screen)
        {
            screenManager.StartScreen(screen, false);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Event handler

        public override bool HandleEvent(Event evt)
        {
            return screenManager.HandleEvent(evt);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public Controller ChildController
        {
            get { return childController; }
        }

        public Controller ParentController
        {
            get { return parentController; }
        }

        protected RootController RootController()
        {
            return Application.RootController();
        }

        protected CConsole Console()
        {
            return RootController().console;
        }

        #endregion
    }
}