using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using Microsoft.Xna.Framework;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core.Assets;
using BomberEngine.Core.Visual;
using Microsoft.Xna.Framework.Graphics;

namespace BomberEngine.Game
{
    public abstract class Application
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

        private int width;
        private int height;

        public Application(int width, int height)
        {
            this.width = width;
            this.height = height;

            context = new Context();
            updatables = new UpdatableList();
            drawables = new DrawableList();
        }

        //////////////////////////////////////////////////////////////////////////////

        protected TimerManager CreateTimerManager()
        {
            return new TimerManager();
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

        public void Draw(GraphicsDevice graphicsDevice)
        {
            context.Begin(graphicsDevice, width, height);
            drawables.Draw(context);
            context.End();
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
