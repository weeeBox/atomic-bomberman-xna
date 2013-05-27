using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Visual.UI;
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

        public int width;
        public int height;

        private View mRootView;
        private View mFocusedView;

        private TimerManager timerManager;

        private UpdatableList updatableList;
        private DrawableList drawableList;

        private KeyboardListenerList keyboardListeners;
        private GamePadListenerList gamePadListeners;

        public ScreenManager screenManager;

        protected bool allowsDrawPrevious;
        protected bool allowsUpdatePrevious;

        public Screen()
            : this(Application.GetWidth(), Application.GetHeight())
        {
        }

        public Screen(int width, int height)
        {
            this.width = width;
            this.height = height;

            timerManager = new TimerManager();
            keyboardListeners = new KeyboardListenerList();
            gamePadListeners = new GamePadListenerList();

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
            OnStop();
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

        protected void ScheduleTimer(TimerCallback callback, float delay)
        {
            ScheduleTimer(callback, delay, false);
        }

        protected void ScheduleTimer(TimerCallback callback, float delay, bool repeated)
        {
            timerManager.Schedule(callback, delay, repeated);
        }

        protected void CancelTimer(TimerCallback callback)
        {
            timerManager.Cancel(callback);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Event handler

        public override bool HandleEvent(Event evt)
        {
            if (evt.code == Event.GAMEPAD)
            {
                GamePadEvent gamePadEvt = evt as GamePadEvent;
                switch (gamePadEvt.state)
                {
                    case GamePadEvent.PRESSED:
                    {
                        if (gamePadListeners.OnButtonPressed(gamePadEvt.arg)) return true;
                        break;
                    }
                    case GamePadEvent.REPEATED:
                    {
                        if (gamePadListeners.OnButtonReleased(gamePadEvt.arg)) return true;
                        break;
                    }
                    case GamePadEvent.RELEASED:
                    {
                        if (gamePadListeners.OnButtonReleased(gamePadEvt.arg)) return true;
                        break;
                    }
                }

                if (mFocusedView != null)
                {
                    if (mFocusedView.HandleEvent(evt)) return true;
                }

                switch (gamePadEvt.state)
                {
                    case GamePadEvent.PRESSED:
                    case GamePadEvent.REPEATED:
                        return TryMoveFocus(gamePadEvt.arg.button);
                }
            }
            else if (evt.code == Event.KEYBOARD)
            {
                KeyboardEvent keyEvent = evt as KeyboardEvent;
                switch (keyEvent.state)
                {
                    case KeyboardEvent.PRESSED:
                    {
                        if (keyboardListeners.OnKeyPressed(keyEvent.key)) return true;
                        break;
                    }
                    case KeyboardEvent.REPEATED:
                    {
                        if (keyboardListeners.OnKeyRepeated(keyEvent.key)) return true;
                        break;
                    }
                    case KeyboardEvent.RELEASED:
                    {
                        if (keyboardListeners.OnKeyReleased(keyEvent.key)) return true;
                        break;
                    }
                }

                if (mFocusedView != null)
                {
                    if (mFocusedView.HandleEvent(evt)) return true;
                }

                switch (keyEvent.state)
                {
                    case KeyboardEvent.PRESSED:
                    case KeyboardEvent.REPEATED:
                        return TryMoveFocus(keyEvent.key);
                }
            }

            return base.HandleEvent(evt);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region KeyboardListeners
        
        public bool AddKeyboardListener(IKeyboardListener listener)
        {
            return keyboardListeners.Add(listener);
        }

        public bool RemoveKeyboardListener(IKeyboardListener listener)
        {
            return keyboardListeners.Remove(listener);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region GamePadListeners

        public bool AddGamePadListener(IGamePadListener listener)
        {
            return gamePadListeners.Add(listener);
        }

        public bool RemoveGamePadListener(IGamePadListener listener)
        {
            return gamePadListeners.Remove(listener);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Focus

        private bool TryMoveFocus(Keys key)
        {
            FocusDirection direction = FindFocusDirection(key);
            if (direction != FocusDirection.None)
            {   
                return TryMoveFocus(direction);
            }
            return false;
        }

        private bool TryMoveFocus(Buttons button)
        {
            FocusDirection direction = FindFocusDirection(button);
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

        protected virtual FocusDirection FindFocusDirection(Keys key)
        {
            switch (key)
            {
                case Keys.Down:
                    return FocusDirection.Down;
                case Keys.Up:
                    return FocusDirection.Up;
                case Keys.Left:
                    return FocusDirection.Left;
                case Keys.Right:
                    return FocusDirection.Right;
            }

            return FocusDirection.None;
        }

        protected virtual FocusDirection FindFocusDirection(Buttons button)
        {
            switch (button)
            {
                case Buttons.DPadDown:
                case Buttons.LeftThumbstickDown:
                    return FocusDirection.Down;
                case Buttons.DPadUp:
                case Buttons.LeftThumbstickUp:
                    return FocusDirection.Up;
                case Buttons.DPadLeft:
                case Buttons.LeftThumbstickLeft:
                    return FocusDirection.Left;
                case Buttons.DPadRight:
                case Buttons.LeftThumbstickRight:
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
            get { return Application.RootController.CurrentController; }
        }

        #endregion
    }
}
