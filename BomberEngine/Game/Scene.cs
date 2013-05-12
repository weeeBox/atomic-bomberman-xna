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
    public class Scene : View
    {
        public View rootView;

        private TimerManager timerManager;

        private UpdatableList updatableList;
        private DrawableList drawableList;

        private KeyboardListenerList keyboardListeners;
        private IGamePadListenerList gamePadListeners;

        public SceneManager sceneManager;
        public SceneListener listener;

        protected bool m_allowsDrawPrevious;
        protected bool m_allowsUpdatePrevious;

        public Scene()
            : this(Application.GetWidth(), Application.GetHeight())
        {
        }

        public Scene(int width, int height)
            : base(width, height)
        {
            timerManager = new TimerManager();
            keyboardListeners = new KeyboardListenerList();
            gamePadListeners = new IGamePadListenerList();

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
                listener.OnSceneStarted(this);
            }
        }

        private void NotifySuspended()
        {
            if (listener != null)
            {
                listener.OnSceneSuspended(this);
            }
        }

        private void NotifyResumed()
        {
            if (listener != null)
            {
                listener.OnSceneResumed(this);
            }
        }

        private void NotifyStoped()
        {
            if (listener != null)
            {
                listener.OnSceneStoped(this);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region KeyboardListener

        public override void KeyPressed(Keys key)
        {
            keyboardListeners.KeyPressed(key);
        }

        public override void KeyReleased(Keys key)
        {
            keyboardListeners.KeyReleased(key);
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

        public override void ButtonPressed(ButtonEvent e)
        {
            gamePadListeners.ButtonPressed(e);
        }

        public override void ButtonReleased(ButtonEvent e)
        {
            gamePadListeners.ButtonReleased(e);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Scenes management

        protected void StartScene(Scene scene)
        {
            sceneManager.StartScene(scene);
        }

        protected void StartNextScene(Scene scene)
        {
            sceneManager.StartScene(scene, false);
        }

        private void RemoveFromContainer()
        {
            if (sceneManager == null)
            {
                throw new InvalidOperationException("Scene container is not set");
            }
            sceneManager.RemoveScene(this);
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
