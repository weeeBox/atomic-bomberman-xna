using System;
using System.Collections.Generic;

namespace BomberEngine
{
    public class Controller : BaseElement, IDestroyable
    {
        private const int StopExit = -1;

        protected ScreenManager screenManager;

        protected Controller childController;
        protected Controller parentController;

        public int exitCode;
        public Object exitData;

        public Controller()
        {   
            screenManager = new ScreenManager(this);
        }

        public virtual void Destroy()
        {
            RemoveDebugViews();

            screenManager.Destroy();
            exitData = null;
            exitCode = 0;
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
            Stop(StopExit);
        }

        public void Stop(int exitCode, Object exitData = null)
        {
            this.exitCode = exitCode;
            this.exitData = exitData;

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

            Destroy();
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

        public bool IsExiting
        {
            get { return exitCode == StopExit; }
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

        public void StartPopup(Popup popup)
        {
            screenManager.StartPopup(popup);
        }

        public void StartScreen(Screen screen)
        {   
            screenManager.StartScreen(screen, true);
        }

        public void StartNextScreen(Screen screen)
        {
            screenManager.StartScreen(screen, false);
        }

        protected Screen FindScreen(int id)
        {
            return screenManager.FindScreen(id);
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

        protected virtual RootController GetRootController()
        {
            return Application.RootController();
        }

        protected virtual CConsole GetConsole()
        {
            return GetRootController().Console;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Screen stack

        internal void OnEmptyScreenStack(ScreenManager manager)
        {
            Stop();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        private List<View> m_debugViews;

        [System.Diagnostics.Conditional("DEBUG_VIEW")]
        public void AddDebugView(View view)
        {
            Application.RootController().AddDebugView(view);
            if (m_debugViews == null)
            {
                m_debugViews = new List<View>();
            }
            m_debugViews.Add(view);
        }

        [System.Diagnostics.Conditional("DEBUG_VIEW")]
        public void RemoveDebugView(View view)
        {
            Application.RootController().RemoveDebugView(view);
            m_debugViews.Remove(view);
        }

        [System.Diagnostics.Conditional("DEBUG_VIEW")]
        protected void RemoveDebugViews()
        {
            if (m_debugViews != null)
            {
                foreach (View view in m_debugViews)
                {
                    Application.RootController().RemoveDebugView(view);
                }
                m_debugViews.Clear();
            }
        }
    }
}