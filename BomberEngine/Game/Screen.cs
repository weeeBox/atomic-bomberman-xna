using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Visual.UI;

namespace BomberEngine.Game
{
    public class Screen : View
    {
        public View rootView;

        private TimerManager timerManager;

        private UpdatableList updatableList;
        private DrawableList drawableList;

        private KeyboardListenerList keyboardListeners;
        private GamePadListenerList gamePadListeners;

        public ScreenManager screenManager;
        public ScreenListener listener;

        protected bool m_allowsDrawPrevious;
        protected bool m_allowsUpdatePrevious;

        public Screen()
            : this(Application.GetWidth(), Application.GetHeight())
        {
        }

        public Screen(int width, int height)
            : base(width, height)
        {
            timerManager = new TimerManager();
            keyboardListeners = new KeyboardListenerList();
            gamePadListeners = new GamePadListenerList();

            updatableList = UpdatableList.Null;
            drawableList = DrawableList.Null;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        internal void Start()
        {
            OnStart();
            NotifyStarted();
        }

        internal void Suspend()
        {
            OnSuspend();
            NotifySuspended();
        }

        internal void Resume()
        {
            OnResume();
            NotifyResumed();
        }

        internal void Stop()
        {
            OnStop();
            NotifyStoped();
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
            base.Update(delta);

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
            base.Draw(context);
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

        protected Timer ScheduleTimer(TimerCallback callback, float delay)
        {
            return ScheduleTimer(callback, delay, false);
        }

        protected Timer ScheduleTimer(TimerCallback callback, float delay, bool repeated)
        {
            return timerManager.Schedule(callback, delay, repeated);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Notifications

        private void NotifyStarted()
        {
            if (listener != null)
            {
                listener.OnScreenStarted(this);
            }
        }

        private void NotifySuspended()
        {
            if (listener != null)
            {
                listener.OnScreenSuspended(this);
            }
        }

        private void NotifyResumed()
        {
            if (listener != null)
            {
                listener.OnScreenResumed(this);
            }
        }

        private void NotifyStoped()
        {
            if (listener != null)
            {
                listener.OnScreenStoped(this);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region KeyboardListener

        public override bool OnKeyPressed(Keys key)
        {
            return keyboardListeners.OnKeyPressed(key);
        }

        public override bool OnKeyReleased(Keys key)
        {
            return keyboardListeners.OnKeyReleased(key);
        }

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

        public override bool OnButtonPressed(ButtonEvent e)
        {
            return gamePadListeners.OnButtonPressed(e);
        }

        public override bool OnButtonReleased(ButtonEvent e)
        {
            return gamePadListeners.OnButtonReleased(e);
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

        public bool allowsDrawPrevious
        {
            get { return m_allowsDrawPrevious; }
            protected set { m_allowsDrawPrevious = value; }
        }

        public bool allowsUpdatePrevious
        {
            get { return m_allowsUpdatePrevious; }
            protected set { m_allowsUpdatePrevious = value; }
        }

        #endregion
    }
}
