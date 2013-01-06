using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;

namespace BomberEngine.Game
{
    public class Scene : Updatable, Drawable, InputListener, TimerManagerListener
    {
        private TimerManager timerManager;

        private UpdatableList updatablesList;

        private DrawableList drawableList;

        private ScenesContainer sceneContainer;

        private SceneListener listener;

        private bool fullscreen;

        public Scene()
            : this(true)
        {
        }

        public Scene(bool fullscreen)
        {   
            this.fullscreen = fullscreen;

            timerManager = new TimerManager(this);
            updatablesList = new UpdatableList();
            drawableList = new DrawableList();
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

        internal void PushBack()
        {
            OnPushBack();
            NotifyPushedBack();
        }

        internal void BringFront()
        {
            OnBringFront();
            NotifyBringFront();
        }

        internal void Stop()
        {   
            OnStop();
            NotifyStoped();
            RemoveFromContainer();
        }

        protected void Finish()
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

        public void Update(float delta)
        {
            updatablesList.Update(delta);
        }

        protected void AddUpdatabled(Updatable updatable)
        {
            updatablesList.Add(updatable);
        }

        protected void RemoveUpdatable(Updatable updatable)
        {
            updatablesList.Remove(updatable);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Drawable

        public void Draw(Context context)
        {
            drawableList.Draw(context);
        }

        protected void AddDrawable(Drawable drawable)
        {
            drawableList.Add(drawable);
        }

        protected void RemoveDrawable(Drawable drawable)
        {
            drawableList.Remove(drawable);
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
            return TimerManager.Schedule(callback, delay, repeated);
        }

        public void OnTimerAdded(TimerManager manager, Timer timer)
        {
            if (manager.TimersCount == 1)
            {
                updatablesList.Add(manager);
            }
        }

        public void OnTimerRemoved(TimerManager manager, Timer timer)
        {
            if (manager.TimersCount == 0)
            {
                updatablesList.Remove(manager);
            }
        }

        private TimerManager TimerManager
        {
            get
            {
                return timerManager;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region InputListener methods

        public void KeyPressed(Keys key)
        {
        }

        public void KeyReleased(Keys key)
        {
        }

        public void ButtonPressed(ButtonEvent e)
        {
        }

        public void ButtonReleased(ButtonEvent e)
        {
        }

        public void GamePadConnected(int playerIndex)
        {
        }

        public void GamePadDisconnected(int playerIndex)
        {
        }

        public void PointerMoved(int x, int y, int fingerId)
        {
        }

        public void PointerPressed(int x, int y, int fingerId)
        {
        }

        public void PointerDragged(int x, int y, int fingerId)
        {
        }

        public void PointerReleased(int x, int y, int fingerId)
        {
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

        private void NotifyPushedBack()
        {
            if (listener != null)
            {
                listener.OnScenePushedBack(this);
            }
        }

        private void NotifyBringFront()
        {
            if (listener != null)
            {
                listener.OnSceneBringFront(this);
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

        #region Scene container

        private void RemoveFromContainer()
        {
            if (sceneContainer == null)
            {
                throw new InvalidOperationException("Scene container is not set");
            }
            sceneContainer.RemoveScene(this);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public bool IsFullscreen
        {
            get { return fullscreen; }
        }

        public SceneListener Listener
        {
            get { return listener; }
            set { listener = value; }
        }

        public ScenesContainer SceneContainer
        {
            get { return sceneContainer; }
            set { sceneContainer = value; }
        }

        #endregion
    }
}
