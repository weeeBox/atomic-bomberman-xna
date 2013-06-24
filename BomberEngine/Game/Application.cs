using System;
using BomberEngine.Core;
using BomberEngine.Core.Assets;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual;
using BomberEngine.Debugging;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BomberEngine.Game
{
    public abstract class Application
    {
        public static Application sharedApplication;

        private RootController rootController;

        private Context context;

        private bool started;
        private bool stoped;

        private DelayedCallManager timerManager;
        private InputManager inputManager;
        private AssetManager assetManager;

        private UpdatableList updatables;
        private DrawableList drawables;

        private int width;
        private int height;

        private double currentTime;

        public Application(int width, int height)
        {
            this.width = width;
            this.height = height;

            sharedApplication = this;

            context = new Context();
            updatables = new UpdatableList();
            drawables = new DrawableList();
        }

        //////////////////////////////////////////////////////////////////////////////

        protected DelayedCallManager CreateTimerManager()
        {
            return new DelayedCallManager();
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
            AddUpdatable(inputManager);

            assetManager = CreateAssetManager();

            rootController = CreateRootController();
            AddGameObject(rootController);
            
            started = true;

            inputManager.SetInputListener(rootController);
            rootController.Start();

            OnStart();
        }

        public void Stop()
        {
            stoped = true;
        }

        public void RunStop()
        {
            if (!started)
            {
                throw new InvalidOperationException("Application not started");
            }

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
            currentTime += delta;
            updatables.Update(delta);
            timerManager.Update(delta);
        }

        public void Draw(GraphicsDevice graphicsDevice)
        {
            context.Begin(graphicsDevice, width, height);
            drawables.Draw(context);
            context.End();
        }

        //////////////////////////////////////////////////////////////////////////////

        public static void ScheduleCall(DelayedCallback callback)
        {
            ScheduleCall(callback, 0.0f);
        }

        public static void ScheduleCall(DelayedCallback callback, float delay)
        {
            ScheduleCall(callback, delay, false);
        }

        public static void ScheduleCall(DelayedCallback callback, float delay, Boolean repeated)
        {
            ScheduleCall(callback, delay, repeated ? 0 : 1);
        }

        public static void ScheduleCall(DelayedCallback callback, float delay, int numRepeats)
        {
            sharedApplication.timerManager.Schedule(callback, delay, numRepeats);
        }

        public static void CancelCall(DelayedCallback callback)
        {
            sharedApplication.timerManager.Cancel(callback);
        }

        public static void CancelAllCalls(Object target)
        {
            sharedApplication.timerManager.CancelAll(target);
        }

        public static void CancelAllCalls()
        {
            sharedApplication.timerManager.CancelAll();
        }

        //////////////////////////////////////////////////////////////////////////////

        protected void AddUpdatable(IUpdatable updatable)
        {
            updatables.Add(updatable);
        }

        protected void RemoveUpdatable(IUpdatable updatable)
        {
            updatables.Remove(updatable);
        }

        protected void AddDrawable(IDrawable drawable)
        {
            drawables.Add(drawable);
        }

        protected void RemoveDrawable(IDrawable drawable)
        {
            drawables.Remove(drawable);
        }

        protected void AddGameObject(BaseElement obj)
        {
            AddUpdatable(obj);
            AddDrawable(obj);
        }

        protected void RemoveGameObject(BaseElement obj)
        {
            RemoveUpdatable(obj);
            RemoveDrawable(obj);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Getters/Setters

        public static AssetManager Assets()
        {
            return sharedApplication.assetManager;
        }

        public static InputManager Input()
        {
            return sharedApplication.inputManager;
        }

        public static RootController RootController()
        {
            return sharedApplication.rootController;
        }

        public bool IsRunning()
        {
            return !stoped;
        }

        public static int GetWidth()
        {
            return sharedApplication.width;
        }

        public static int GetHeight()
        {
            return sharedApplication.height;
        }

        public static double CurrentTime()
        {
            return sharedApplication.currentTime;
        }

        #endregion
    }
}
