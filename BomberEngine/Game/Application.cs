using System;
using BomberEngine.Core;
using BomberEngine.Core.Assets;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual;
using BomberEngine.Debugging;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Native;
using BomberEngine.Core.Events;

namespace BomberEngine.Game
{
    public struct ApplicationInfo
    {
        public INativeInterface nativeInterface;

        public int width;
        public int height;
        public int realWidth;
        public int realHeight;

        public ApplicationInfo(int width, int height)
        {
            this.width = width;
            this.height = height;
            realWidth = width;
            realHeight = height;

            nativeInterface = null;
        }
    }

    public abstract class Application
    {
        public static Application sharedApplication;

        private RootController rootController;

        protected Context context;

        private bool started;
        private bool stoped;

        private TimerManager timerManager;
        private InputManager inputManager;
        private AssetManager assetManager;
        private NotificationCenter notifications;

        private UpdatableList updatables;
        private DrawableList drawables;

        private INativeInterface nativeInterface;
        private int width;
        private int height;
        private int realWidth;
        private int realHeight;

        private double currentTime;

        public Application(ApplicationInfo info)
        {
            nativeInterface = info.nativeInterface;
            width = info.width;
            height = info.height;
            realWidth = info.realWidth;
            realHeight = info.realHeight;

            sharedApplication = this;

            context = new Context();
            updatables = new UpdatableList(1);
            drawables = new DrawableList(1);
        }

        //////////////////////////////////////////////////////////////////////////////

        protected TimerManager CreateTimerManager()
        {
            return new TimerManager();
        }

        private NotificationCenter CreateNotifications(TimerManager timerManager)
        {
            return new NotificationCenter(timerManager);
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
            notifications = CreateNotifications(timerManager);

            inputManager = CreateInputManager();
            AddUpdatable(inputManager);

            assetManager = CreateAssetManager();

            rootController = CreateRootController();
            AddGameObject(rootController);
            
            started = true;
            OnStart();

            inputManager.SetInputListener(rootController);
            rootController.Start();
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
            context.Begin(graphicsDevice);
            drawables.Draw(context);
            context.End();
        }

        //////////////////////////////////////////////////////////////////////////////

        public static Timer ScheduleTimer(TimerCallback callback)
        {
            return ScheduleTimer(callback, 0.0f);
        }

        public static Timer ScheduleTimer(TimerCallback callback, float delay)
        {
            return ScheduleTimer(callback, delay, false);
        }

        public static Timer ScheduleTimer(TimerCallback callback, float delay, Boolean repeated)
        {
            return ScheduleTimer(callback, delay, repeated ? 0 : 1);
        }

        public static Timer ScheduleTimerOnce(TimerCallback callback)
        {
            return ScheduleTimerOnce(callback, 0.0f);
        }

        public static Timer ScheduleTimerOnce(TimerCallback callback, float delay)
        {
            return ScheduleTimerOnce(callback, delay, false);
        }

        public static Timer ScheduleTimerOnce(TimerCallback callback, float delay, Boolean repeated)
        {
            return ScheduleTimerOnce(callback, delay, repeated ? 0 : 1);
        }

        public static Timer ScheduleTimer(TimerCallback callback, float delay, int numRepeats)
        {
            return sharedApplication.timerManager.Schedule(callback, delay, numRepeats);
        }

        public static Timer ScheduleTimerOnce(TimerCallback callback, float delay, int numRepeats)
        {
            return sharedApplication.timerManager.ScheduleOnce(callback, delay, numRepeats);
        }

        public static void CancelTimer(TimerCallback callback)
        {
            sharedApplication.timerManager.Cancel(callback);
        }

        public static void CancelAllTimers(Object target)
        {
            sharedApplication.timerManager.CancelAll(target);
        }

        public static void CancelAllTimers()
        {
            sharedApplication.timerManager.CancelAll();
        }

        //////////////////////////////////////////////////////////////////////////////

        public static void RegisterNotification(String name, NotificationDelegate del)
        {
            sharedApplication.notifications.Register(name, del);
        }

        public static void UnregisterNotification(String name, NotificationDelegate del)
        {
            sharedApplication.notifications.Unregister(name, del);
        }

        public static void UnregisterNotifications(NotificationDelegate del)
        {
            sharedApplication.notifications.UnregisterAll(del);
        }

        public static void UnregisterNotifications(Object target)
        {
            sharedApplication.notifications.UnregisterAll(target);
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

        #region Native interface

        public static void SetWindowTitle(String title)
        {
            sharedApplication.nativeInterface.SetWindowTitle(title);
        }

        #endregion

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

        public static TimerManager TimerManager()
        {
            return sharedApplication.timerManager;
        }

        public static NotificationCenter Notifications()
        {
            return sharedApplication.notifications;
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

        public static int GetRealWidth()
        {
            return sharedApplication.realWidth;
        }

        public static int GetRealHeight()
        {
            return sharedApplication.realWidth;
        }

        public static double CurrentTime()
        {
            return sharedApplication.currentTime;
        }

        #endregion
    }
}
