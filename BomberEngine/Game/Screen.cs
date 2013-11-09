using System;
using System.Collections.Generic;

namespace BomberEngine
{
    public class Screen : BaseElement, IDestroyable, IFocusManager
    {
        public float width;
        public float height;

        private View mRootView;
        private View mFocusedView;
        private View mFocusLockView;

        private ITimerManager timerManager;

        private UpdatableList updatables;
        private DrawableList drawables;
        private EventHandlerList eventHandlers;

        public ScreenManager screenManager;

        private bool allowsDrawPrevious;
        private bool allowsUpdatePrevious;
        private bool m_finishOnCancel;

        private Button cancelButton;
        private ButtonDelegate cancelButtonDelegate;

        private Button confirmButton;
        private ButtonDelegate confirmButtonDelegate;

        internal static Screen current;

        public Screen(int id = 0)
            : this(Application.GetWidth(), Application.GetHeight())
        {
            this.id = id;
        }

        public Screen(float width, float height)
        {
            this.width = width;
            this.height = height;

            timerManager = TimerManager.Null;

            updatables = UpdatableList.Null;
            drawables = DrawableList.Null;
            eventHandlers = EventHandlerList.Null;

            m_finishOnCancel = true;
            mRootView = CreateRootView();
        }

        public virtual void Destroy()
        {
            timerManager.Destroy();
            updatables.Destroy();
            UnregisterNotifications();
            RemoveDebugViews();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Root view

        protected View CreateRootView()
        {
            return new View(0, 0, width, height);
        }

        public void AddView(View view)
        {
            RootView.AddView(view);
        }

        public void AddView(View view, float x, float y)
        {
            AddView(view);
            view.x = x;
            view.y = y;
        }

        internal void OnViewRemoved(View view)
        {
            if (view == mFocusedView)
            {
                Assert.True(view.focused);
                mFocusedView = null;
            }
            else if (view == mFocusLockView)
            {
                Assert.True(view.focused);
                mFocusLockView = null;
            }
            else if (view == mRootView)
            {
                mRootView = null;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        internal void Start()
        {
            OnStart();

            if (mRootView != null && mFocusedView == null)
            {
                TryMoveFocus(FocusDirection.Down);
            }
        }

        internal void Suspend()
        {
            OnSuspend();
        }

        internal void Resume()
        {
            OnResume();
        }

        internal void Stop()
        {   
            OnStop();
            Destroy();
        }

        public void Finish()
        {
            RemoveFromContainer();
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnSuspend()
        {
        }

        protected virtual void OnResume()
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
            mRootView.Update(delta);

            updatables.Update(delta);
            timerManager.Update(delta);
        }

        public void AddUpdatable(IUpdatable updatable)
        {
            if (updatables.IsNull())
            {
                updatables = new UpdatableList();
            }
            updatables.Add(updatable);
        }

        public void RemoveUpdatable(IUpdatable updatable)
        {
            if (updatables.Count() > 0)
            {
                updatables.Remove(updatable);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Drawable

        public override void Draw(Context context)
        {
            mRootView.Draw(context);
            drawables.Draw(context);
        }

        public void AddDrawable(IDrawable drawable)
        {
            if (drawables.IsNull())
            {
                drawables = new DrawableList();
            }
            drawables.Add(drawable);
        }

        public void RemoveDrawable(IDrawable drawable)
        {
            if (drawables.Count() > 0)
            {
                drawables.Remove(drawable);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region TimerManager

        public void ScheduleTimer(TimerCallback1 callback, float delay = 0.0f, bool repeated = false)
        {
            ScheduleTimer(callback, delay, repeated ? 0 : 1);
        }

        public void ScheduleTimer(TimerCallback2 callback, float delay = 0.0f, bool repeated = false)
        {
            ScheduleTimer(callback, delay, repeated ? 0 : 1);
        }

        public void ScheduleTimer(TimerCallback1 callback, float delay, int numRepeats)
        {
            if (timerManager == TimerManager.Null)
            {
                timerManager = new TimerManager();
            }
            timerManager.Schedule(callback, delay, numRepeats);
        }

        public void ScheduleTimer(TimerCallback2 callback, float delay, int numRepeats)
        {
            if (timerManager == TimerManager.Null)
            {
                timerManager = new TimerManager();
            }
            timerManager.Schedule(callback, delay, numRepeats);
        }

        public void ScheduleTimerOnce(TimerCallback1 callback, float delay = 0.0f, bool repeated = false)
        {
            ScheduleTimerOnce(callback, delay, repeated ? 0 : 1);
        }

        public void ScheduleTimerOnce(TimerCallback2 callback, float delay = 0.0f, bool repeated = false)
        {
            ScheduleTimerOnce(callback, delay, repeated ? 0 : 1);
        }

        public void ScheduleTimerOnce(TimerCallback1 callback, float delay, int numRepeats)
        {
            if (timerManager == TimerManager.Null)
            {
                timerManager = new TimerManager();
            }
            timerManager.ScheduleOnce(callback, delay, numRepeats);
        }

        public void ScheduleTimerOnce(TimerCallback2 callback, float delay, int numRepeats)
        {
            if (timerManager == TimerManager.Null)
            {
                timerManager = new TimerManager();
            }
            timerManager.ScheduleOnce(callback, delay, numRepeats);
        }

        public void CancelTimer(TimerCallback1 callback)
        {
            timerManager.Cancel(callback);
        }

        public void CancelTimer(TimerCallback2 callback)
        {
            timerManager.Cancel(callback);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Event handler

        public override bool HandleEvent(Event evt)
        {
            if (eventHandlers.HandleEvent(evt))
            {
                return true;
            }

            if (evt.code == Event.KEY)
            {
                if (mFocusLockView != null && mFocusLockView.HandleEvent(evt)) return true;
                if (mFocusedView != null && mFocusedView.HandleEvent(evt)) return true;

                KeyEvent keyEvent = evt as KeyEvent;
                if (keyEvent.state == KeyState.Pressed)
                {
                    if (keyEvent.IsConfirmKey())
                    {
                        if (OnConfirmPressed(keyEvent.arg))
                        {
                            return true;
                        }
                    }

                    if (keyEvent.IsCancelKey())
                    {
                        if (OnCancelPressed(keyEvent.arg))
                        {
                            return true;
                        }
                    }
                }

                switch (keyEvent.state)
                {
                    case KeyState.Pressed:
                    case KeyState.Repeated:
                        return TryMoveFocus(keyEvent.arg.key);
                }
            }

            return base.HandleEvent(evt);
        }

        protected void AddEventHandler(IEventHandler handler)
        {
            if (eventHandlers.IsNull())
            {
                eventHandlers = new EventHandlerList();
            }

            eventHandlers.Add(handler);
        }

        protected void RemoveEventHandler(IEventHandler handler)
        {
            if (!eventHandlers.IsNull())
            {
                eventHandlers.Remove(handler);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Focus

        private bool TryMoveFocus(KeyCode key)
        {
            FocusDirection direction = FindFocusDirection(key);
            if (direction != FocusDirection.None)
            {   
                return TryMoveFocus(direction);
            }
            return false;
        }

        private bool TryMoveFocus(FocusDirection direction)
        {
            View view = FindFocusView(direction);
            if (view != null)
            {
                return FocusView(view);
            }

            return false;
        }

        private View FindFocusView(FocusDirection direction)
        {   
            if (mFocusLockView != null)
            {
                return mFocusLockView.FindFocusView(this, mFocusedView, direction);
            }

            if (mFocusedView != null)
            {
                View parentView = mFocusedView.Parent();
                return parentView.FindFocusView(this, mFocusedView, direction);
            }
            
            return mRootView.FindFocusView(this, direction);
        }

        public bool FocusView(View view)
        {
            if (view != mFocusedView)
            {
                if (mFocusedView != null)
                {
                    mFocusedView.blur();
                }

                if (view != null)
                {
                    view = view.FindFocusView(this, FocusDirection.Down);
                    if (view != null)
                    {
                        view.focus();
                    }
                }

                mFocusedView = view;
                return mFocusedView != null;
            }

            return false;
        }

        protected virtual FocusDirection FindFocusDirection(KeyCode key)
        {
            switch (key)
            {
                case KeyCode.Down:
                    return FocusDirection.Down;
                case KeyCode.Up:
                    return FocusDirection.Up;
                case KeyCode.Left:
                    return FocusDirection.Left;
                case KeyCode.Right:
                    return FocusDirection.Right;
            }

            return FocusDirection.None;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region IFocusManager

        public void LockFocus(View view)
        {
            Assert.True(mFocusLockView == null || mFocusLockView == view);
            mFocusLockView = view;
        }

        public void UnlockFocus(View view)
        {
            Assert.True(mFocusLockView == view);
            mFocusLockView = null;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Screens management

        protected void StartScreen(Screen screen)
        {
            screenManager.StartScreen(screen);
        }

        protected void StartNextScreen(Screen screen)
        {
            screenManager.StartScreen(screen, false);
        }

        private void RemoveFromContainer()
        {
            if (screenManager == null)
            {
                throw new InvalidOperationException("Screen container is not set");
            }
            screenManager.RemoveScreen(this);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public static Screen Current
        {
            get { return current; }
        }

        public bool AllowsDrawPrevious
        {
            get { return allowsDrawPrevious; }
            protected set { allowsDrawPrevious = value; }
        }

        public bool AllowsUpdatePrevious
        {
            get { return allowsUpdatePrevious; }
            protected set { allowsUpdatePrevious = value; }
        }

        protected View RootView
        {
            get { return mRootView; }
        }

        public View focusedView
        {
            get { return mFocusedView; }
        }

        protected Controller CurrentController
        {
            get { return Application.RootController().GetCurrentController(); }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Back key/button

        public void SetCancelButton(Button button)
        {
            cancelButton = button;

            if (button != null)
            {
                cancelButtonDelegate = button.buttonDelegate;
                button.buttonDelegate = OnCancelButtonPress;
            }
            else
            {
                cancelButtonDelegate = null;
            }
        }

        public void SetConfirmButton(Button button)
        {
            confirmButton = button;

            if (button != null)
            {
                confirmButtonDelegate = button.buttonDelegate;
                button.buttonDelegate = OnConfirmButtonPress;
            }
            else
            {
                confirmButtonDelegate = null;
            }
        }

        protected virtual void OnCancelButtonPress(Button button)
        {
            Finish();
            if (cancelButtonDelegate != null)
            {
                cancelButtonDelegate(button);
            }
        }

        protected virtual void OnConfirmButtonPress(Button button)
        {
            if (confirmButtonDelegate != null)
            {
                confirmButtonDelegate(button);
            }
        }

        protected virtual bool OnConfirmPressed(KeyEventArg arg)
        {
            bool result = confirmButton != null;
            if (result)
            {
                ScheduleTimer(HandleConfirm);
                return true;
            }

            return false;
        }

        protected virtual bool OnCancelPressed(KeyEventArg arg)
        {
            ScheduleTimer(HandleCancel);
            return true;
        }

        private void HandleConfirm()
        {
            if (confirmButton != null)
            {
                OnConfirmButtonPress(confirmButton);
            }
        }

        private void HandleCancel()
        {
            if (cancelButton != null)
            {
                OnCancelButtonPress(cancelButton);
            }
            else if (IsFinishOnCancel)
            {
                Finish();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public bool IsFinishOnCancel
        {
            get { return m_finishOnCancel; }
            protected set { m_finishOnCancel = value; }
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
