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

        private ScenesContainer sceneContainer;

        private SceneListener listener;

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

        public override void Draw(Context context)
        {
            PreDraw(context);
            drawableList.Draw(context);
            PostDraw(context);
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
