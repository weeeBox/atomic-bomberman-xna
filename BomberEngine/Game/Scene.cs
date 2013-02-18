using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core.Visual;

namespace BomberEngine.Game
{
    public class Scene : DrawableElement
    {
        private TimerManager timerManager;

        private UpdatableList updatablesList;

        private DrawableList drawableList;

        private KeyboardListenerList keyboardListeners;
        private GamePadListenerList gamePadListeners;

        public ScenesManager sceneManager;

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
            updatablesList = new UpdatableList();
            drawableList = new DrawableList();
            keyboardListeners = new KeyboardListenerList();
            gamePadListeners = new GamePadListenerList();
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
            updatablesList.Update(delta);
        }

        public void AddUpdatabled(Updatable updatable)
        {
            updatablesList.Add(updatable);
        }

        public void RemoveUpdatable(Updatable updatable)
        {
            updatablesList.Remove(updatable);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Drawable

        public override void Draw(Context context)
        {
            PreDraw(context);
            drawableList.Draw(context);
            PostDraw(context);
        }

        public void AddDrawable(Drawable drawable)
        {
            drawableList.Add(drawable);
        }

        public void RemoveDrawable(Drawable drawable)
        {
            drawableList.Remove(drawable);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Game Objects

        protected void AddGameObject(GameObject obj)
        {
            AddUpdatabled(obj);
            AddDrawable(obj);
        }

        protected void RemoveGameObject(GameObject obj)
        {
            RemoveUpdatable(obj);
            RemoveDrawable(obj);
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

        public bool AddKeyboardListener(KeyboardListener listener)
        {
            return keyboardListeners.Add(listener);
        }

        public bool RemoveKeyboardListener(KeyboardListener listener)
        {
            return keyboardListeners.Remove(listener);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region GamePadListeners

        public bool AddGamePadListener(GamePadListener listener)
        {
            return gamePadListeners.Add(listener);
        }

        public bool RemoveGamePadListener(GamePadListener listener)
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
