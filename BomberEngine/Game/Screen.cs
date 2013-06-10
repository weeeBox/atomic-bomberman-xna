using System;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual;
using BomberEngine.Debugging;
using BomberEngine.Core.Events;

namespace BomberEngine.Game
{
    public delegate void ScreenDelegate(Screen screen);

    public class Screen : BaseElement
    {
        protected enum FocusDirection
        {
            None, Up, Down, Left, Right
        }

        public float width;
        public float height;

        private View mRootView;
        private View mFocusedView;

        private DelayedCallManager timerManager;

        private UpdatableList updatableList;
        private DrawableList drawableList;

        private IKeyInputListenerList keyInputListeners;

        public ScreenManager screenManager;

        protected bool allowsDrawPrevious;
        protected bool allowsUpdatePrevious;

        public ScreenDelegate onStartDelegate;
        public ScreenDelegate onSuspendDelegate;
        public ScreenDelegate onResumeDelegate;
        public ScreenDelegate onStopDelegate;

        public Screen()
            : this(Application.GetWidth(), Application.GetHeight())
        {
        }

        public Screen(float width, float height)
        {
            this.width = width;
            this.height = height;

            timerManager = new DelayedCallManager();
            keyInputListeners = new IKeyInputListenerList();

            updatableList = UpdatableList.Null;
            drawableList = DrawableList.Null;

            mRootView = CreateRootView();
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

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        internal void Start()
        {
            OnStart();
            NotifyScreenDelegate(onStartDelegate);

            if (mRootView != null)
            {
                TryMoveFocus(FocusDirection.Down);
            }
        }

        internal void Suspend()
        {
            OnSuspend();
            NotifyScreenDelegate(onSuspendDelegate);
        }

        internal void Resume()
        {
            OnResume();
            NotifyScreenDelegate(onResumeDelegate);
        }

        internal void Stop()
        {
            OnStop();
            NotifyScreenDelegate(onStopDelegate);
            RemoveFromContainer();
        }

        public void Finish()
        {
            Stop();
        }

        protected void OnStart()
        {
        }

        protected void OnSuspend()
        {
        }

        protected void OnResume()
        {
        }

        protected void OnPushBack()
        {
        }

        protected void OnBringFront()
        {
        }

        protected void OnStop()
        {
        }

        private void NotifyScreenDelegate(ScreenDelegate screenDelegate)
        {
            if (screenDelegate != null)
            {
                screenDelegate(this);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Updatable

        public override void Update(float delta)
        {
            mRootView.Update(delta);

            updatableList.Update(delta);
            timerManager.Update(delta);
        }

        public void AddUpdatabled(IUpdatable updatable)
        {
            if (updatableList.IsNull())
            {
                updatableList = new UpdatableList();
            }
            updatableList.Add(updatable);
        }

        public void RemoveUpdatable(IUpdatable updatable)
        {
            if (updatableList.Count() > 0)
            {
                updatableList.Remove(updatable);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Drawable

        public override void Draw(Context context)
        {
            mRootView.Draw(context);
            drawableList.Draw(context);
        }

        public void AddDrawable(IDrawable drawable)
        {
            if (drawableList.IsNull())
            {
                drawableList = new DrawableList();
            }
            drawableList.Add(drawable);
        }

        public void RemoveDrawable(IDrawable drawable)
        {
            if (drawableList.Count() > 0)
            {
                drawableList.Remove(drawable);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region TimerManager

        protected void ScheduleTimer(DelayedCallback callback, float delay)
        {
            ScheduleTimer(callback, delay, false);
        }

        protected void ScheduleTimer(DelayedCallback callback, float delay, bool repeated)
        {
            timerManager.Schedule(callback, delay, repeated);
        }

        protected void CancelTimer(DelayedCallback callback)
        {
            timerManager.Cancel(callback);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Event handler

        public override bool HandleEvent(Event evt)
        {   
            if (evt.code == Event.KEY)
            {
                KeyEvent keyEvent = evt as KeyEvent;
                switch (keyEvent.state)
                {
                    case KeyState.Pressed:
                    {
                        if (keyInputListeners.OnKeyPressed(keyEvent.arg)) return true;
                        break;
                    }
                    case KeyState.Repeated:
                    {
                        if (keyInputListeners.OnKeyRepeated(keyEvent.arg)) return true;
                        break;
                    }
                    case KeyState.Released:
                    {
                        if (keyInputListeners.OnKeyReleased(keyEvent.arg)) return true;
                        break;
                    }
                }

                if (mFocusedView != null)
                {
                    if (mFocusedView.HandleEvent(evt)) return true;
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

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region KeyboardListeners

        public bool AddKeyListener(IKeyInputListener listener)
        {
            return keyInputListeners.Add(listener);
        }

        public bool RemoveKeyboardListener(IKeyInputListener listener)
        {
            return keyInputListeners.Remove(listener);
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
            if (mFocusedView != null)
            {
                View parentView = mFocusedView.Parent();
                View view = FindFocusView(parentView, mFocusedView, direction);
                if (view != null)
                {
                    return FocusView(view);
                }
            }
            else
            {
                View view = FindFocusView(mRootView, direction);
                if (view != null)
                {
                    return FocusView(view);
                }
            }

            return false;
        }

        private View FindFocusView(View root, FocusDirection direction)
        {
            if (direction == FocusDirection.Down || direction == FocusDirection.Right)
            {
                for (int i = 0; i < root.ChildCount(); ++i)
                {
                    View child = root.ViewAt(i);
                    View view = FindFocusView(child, direction);
                    if (view != null)
                    {
                        return view;
                    }

                    if (child.CanFocus())
                    {
                        return child;
                    }
                }
            }
            else if (direction == FocusDirection.Up || direction == FocusDirection.Left)
            {
                for (int i = root.ChildCount() - 1; i >= 0; --i)
                {
                    View child = root.ViewAt(i);
                    View view = FindFocusView(child, direction);
                    if (view != null)
                    {
                        return view;
                    }

                    if (child.CanFocus())
                    {
                        return child;
                    }
                }
            }

            return root.focusable ? root : null;
        }

        private View FindFocusView(View root, View current, FocusDirection direction)
        {
            int index = root.IndexOf(current);
            Debug.Assert(index != -1);

            if (direction == FocusDirection.Down || direction == FocusDirection.Right)
            {
                for (int i = index + 1; i < root.ChildCount(); ++i)
                {
                    View child = root.ViewAt(i);
                    View view = FindFocusView(child, direction);
                    if (view != null)
                    {
                        return view;
                    }
                }
            }
            else if (direction == FocusDirection.Up || direction == FocusDirection.Left)
            {
                for (int i = index - 1; i >= 0; --i)
                {
                    View child = root.ViewAt(i);
                    View view = FindFocusView(child, direction);
                    if (view != null)
                    {
                        return view;
                    }
                }
            }

            View parent = root.Parent();
            if (parent != null)
            {
                return FindFocusView(parent, root, direction);
            }

            return null;
        }

        private bool FocusView(View view)
        {
            if (view != mFocusedView)
            {
                if (mFocusedView != null)
                {
                    mFocusedView.blur();
                }

                if (view != null)
                {
                    view.focus();
                }

                mFocusedView = view;
                return true;
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
            get { return Application.RootController().CurrentController; }
        }

        #endregion
    }
}
