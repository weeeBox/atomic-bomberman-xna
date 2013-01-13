using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using Microsoft.Xna.Framework;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core.Assets;

namespace BomberEngine.Game
{
    public abstract class Application : TimerManagerListener
    {   
        private RootController rootController;

        private Context context;

        private bool started;
        private bool stoped;

        private static TimerManager timerManager;
        private static InputManager inputManager;
        private static AssetManager assetManager;

        private UpdatableList updatables;
        private DrawableList drawables;

        public Application(GraphicsDeviceManager graphics)
        {   
            context = new ContextImpl(graphics);
            updatables = new UpdatableList();
            drawables = new DrawableList();
        }

        //////////////////////////////////////////////////////////////////////////////

        protected TimerManager CreateTimerManager()
        {
            return new TimerManager(this);
        }

        protected InputManager CreateInputManager()
        {
            return new InputManager();
        }

        protected abstract AssetManager CreateAssetManager();
        protected abstract RootController CreateRootController();
        
        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        public void Start()
        {
            if (started)
            {
                throw new InvalidOperationException("Application already started");
            }

            timerManager = CreateTimerManager();
            inputManager = CreateInputManager();
            assetManager = CreateAssetManager();
            rootController = CreateRootController();
            
            started = true;
            rootController.Start();

            AddUpdatable(inputManager);
            AddUpdatable(rootController);

            AddDrawable(rootController);

            OnStart();
        }

        public void Stop()
        {
            if (!started)
            {
                throw new InvalidOperationException("Application not started");
            }

            if (stoped)
            {
                throw new InvalidOperationException("Application already stopped");
            }

            stoped = true;
            rootController.Stop();

            OnStop();
        }

        protected virtual void OnStart()
        {

        }

        protected virtual void OnStop()
        {

        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public void Update(float delta)
        {   
            updatables.Update(delta);
        }

        public void Draw()
        {
            drawables.Draw(context);
        }

        //////////////////////////////////////////////////////////////////////////////

        public static Timer ScheduleTimer(TimerCallback callback, float delay)
        {
            return ScheduleTimer(callback, delay, false);
        }

        public static Timer ScheduleTimer(TimerCallback callback, float delay, Boolean repeated)
        {
            return ScheduleTimer(callback, delay, repeated ? 0 : 1);
        }

        public static Timer ScheduleTimer(TimerCallback callback, float delay, int numRepeats)
        {
            return timerManager.Schedule(callback, delay, numRepeats);
        }

        public void OnTimerAdded(TimerManager manager, Timer timer)
        {
            if (manager.TimersCount == 1)
            {
                AddUpdatable(manager);
            }
        }

        public void OnTimerRemoved(TimerManager manager, Timer timer)
        {
            if (manager.TimersCount == 0)
            {
                RemoveUpdatable(manager);
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        protected void AddUpdatable(Updatable updatable)
        {
            updatables.Add(updatable);
        }

        protected void RemoveUpdatable(Updatable updatable)
        {
            updatables.Remove(updatable);
        }

        protected void AddDrawable(Drawable drawable)
        {
            drawables.Add(drawable);
        }

        protected void RemoveDrawable(Drawable drawable)
        {
            drawables.Remove(drawable);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Getters/Setters

        public static AssetManager Assets()
        {
            return assetManager;
        }

        public static InputManager Input()
        {
            return inputManager;
        }

        public bool IsRunning()
        {
            return !stoped;
        }

        #endregion
    }
}
