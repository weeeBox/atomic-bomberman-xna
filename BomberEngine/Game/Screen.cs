using System;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual;
using BomberEngine.Debugging;
using BomberEngine.Core.Events;

namespace BomberEngine.Game
{
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

        private DelayedCallManager callManager;

        private UpdatableList updatables;
        private DrawableList drawables;
        private EventHandlerList eventHandlers;

        public ScreenManager screenManager;

        protected bool allowsDrawPrevious;
        protected bool allowsUpdatePrevious;

        private Button cancelButton;
        private ButtonDelegate cancelButtonDelegate;

        private Button confirmButton;
        private ButtonDelegate confirmButtonDelegate;

        public Screen(int id = 0)
            : this(Application.GetWidth(), Application.GetHeight())
        {
            this.id = id;
        }

        public Screen(float width, float height)
        {
            this.width = width;
            this.height = height;

            callManager = new DelayedCallManager();

            updatables = UpdatableList.Null;
            drawables = DrawableList.Null;
            eventHandlers = EventHandlerList.Null;

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

        public void AddView(View view, float x, float y)
        {
            AddView(view);
            view.x = x;
            view.y = y;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        internal void Start()
        {
            OnStart();

            if (mRootView != null)
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
            callManager.Destroy();
            updatables.Destroy();

            OnStop();
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
            callManager.Update(delta);
        }

        public void AddUpdatable(IUpdatable updatable)
        {
            if (updatables.IsNull())
            {
                updatables = new UpdatableList();
            }
            updatables.Add(updatable);
        }

        public void AddUpdatable(UpdatableDelegate updatableDelegate)
        {
            if (updatables.IsNull())
            {
                updatables = new UpdatableList();
            }
            updatables.Add(updatableDelegate);
        }

        public void RemoveUpdatable(IUpdatable updatable)
        {
            if (updatables.Count() > 0)
            {
                updatables.Remove(updatable);
            }
        }

        public void RemoveUpdatable(UpdatableDelegate updatable)
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

        public void ScheduleCall(DelayedCallback callback)
        {
            ScheduleCall(callback, 0.0f);
        }

        public void ScheduleCall(DelayedCallback callback, float delay)
        {
            ScheduleCall(callback, delay, false);
        }

        public void ScheduleCall(DelayedCallback callback, float delay, Boolean repeated)
        {
            ScheduleCall(callback, delay, repeated ? 0 : 1);
        }

        public void ScheduleCall(DelayedCallback callback, float delay, int numRepeats)
        {
            callManager.Schedule(callback, delay, numRepeats);
        }

        public void CancelCall(DelayedCallback callback)
        {
            callManager.Cancel(callback);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Event handler

        public override bool HandleEvent(Event evt)
        {   
            if (evt.code == Event.KEY)
            {
                if (mFocusedView != null)
                {
                    if (mFocusedView.HandleEvent(evt)) return true;
                }

                KeyEvent keyEvent = evt as KeyEvent;
                if (keyEvent.state == KeyState.Pressed)
                {
                    if (keyEvent.IsKeyPressed(KeyCode.Escape) || 
                        keyEvent.IsKeyPressed(KeyCode.GP_B) || keyEvent.IsKeyPressed(KeyCode.GP_Back))
                    {
                        if (OnCancelPressed(keyEvent.arg))
                        {
                            return true;
                        }
                    }

                    if (keyEvent.IsKeyPressed(KeyCode.Enter) || 
                        keyEvent.IsKeyPressed(KeyCode.GP_A) || keyEvent.IsKeyPressed(KeyCode.GP_Start))
                    {
                        if (OnConfirmPressed(keyEvent.arg))
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

        protected virtual bool OnCancelPressed(KeyEventArg arg)
        {
            if (cancelButton != null)
            {
                OnCancelButtonPress(cancelButton);
            }
            else
            {
                Finish();
            }
            return true;
        }

        protected virtual bool OnConfirmPressed(KeyEventArg arg)
        {
            if (confirmButton != null)
            {
                OnConfirmButtonPress(confirmButton);
                return true;
            }

            return false;
        }

        #endregion
    }
}
