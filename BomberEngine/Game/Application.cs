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
using BomberEngine.Core.Storage;
using BomberEngine.Util;
using BomberEngine.Consoles;
using BomberEngine.Demo;
using BomberEngine.Core.IO;

namespace BomberEngine.Game
{
    public struct ApplicationInfo
    {
        public INativeInterface nativeInterface;

        public int width;
        public int height;
        public int realWidth;
        public int realHeight;
        public String[] args;

        public ApplicationInfo(int width, int height)
        {
            this.width = width;
            this.height = height;
            realWidth = width;
            realHeight = height;

            nativeInterface = null;
            args = new String[0];
        }
    }

    public abstract class Application
    {
        private enum Mode
        {
            Normal,
            Demo,
        }

        public static Application sharedApplication;

        private RootController rootController;

        protected Context context;

        private bool started;
        private bool stoped;

        private CommandLine cmdLine;

        private TimerManager timerManager;
        private InputManager inputManager;
        private AssetManager assetManager;
        private NotificationCenter notifications;
        private SharedStorage sharedStorage;

        private UpdatableList updatables;
        private DrawableList drawables;

        private INativeInterface nativeInterface;
        private int width;
        private int height;
        private int realWidth;
        private int realHeight;

        private float m_currentTime;
        private float m_frameTime;

        private Mode m_mode;
        private DemoCmdEntry m_demoCmdEntry;

        private DemoPlayer m_demoPlayer;
        private DemoRecorder m_demoRecorder;

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

            cmdLine = CreateCommandLine();
            cmdLine.Parse(info.args);
        }

        //////////////////////////////////////////////////////////////////////////////

        protected virtual CommandLine CreateCommandLine()
        {
            m_demoCmdEntry = new DemoCmdEntry();

            CommandLine cmd = new CommandLine();
            cmd.Register(m_demoCmdEntry);
            return cmd;
        }

        protected virtual TimerManager CreateTimerManager()
        {
            return new TimerManager();
        }

        protected virtual NotificationCenter CreateNotifications(TimerManager timerManager)
        {
            return new NotificationCenter(timerManager);
        }

        protected virtual InputManager CreateInputManager()
        {
            return new DefaultInputManager();
        }

        protected virtual SharedStorage CreateSharedStorage(String filename, TimerManager timerManager)
        {
            return new SharedStorage(filename, timerManager);
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

            if (m_demoCmdEntry.FileName != null)
            {
                StartDemo(m_demoCmdEntry.FileName);
            }
            else
            {
                StartNormal();
            }
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

            sharedStorage.Destroy();
            timerManager.Destroy();

            SaveDemo();
        }

        public void RunCrash(Exception e)
        {
            SaveDemo();
        }

        protected virtual void OnStart()
        {

        }

        protected virtual void OnStop()
        {

        }

        private void StartNormal()
        {
            m_mode = Mode.Normal;

            MathHelp.InitRandom();

            CreateDemoRecorder();
            
            timerManager = CreateTimerManager();
            notifications = CreateNotifications(timerManager);
            sharedStorage = CreateSharedStorage("storage", timerManager);

            inputManager = CreateInputManager();
            if (inputManager is IUpdatable)
            {
                AddUpdatable(inputManager as IUpdatable);
            }

            InitDemoRecorder();
            
            assetManager = CreateAssetManager();

            rootController = CreateRootController();
            AddGameObject(rootController);

            started = true;
            OnStart();

            inputManager.AddInputListener(rootController);
            rootController.Start();
        }

        private void StartDemo(String path)
        {
            m_mode = Mode.Demo;

            m_demoPlayer = new DemoPlayer(path);

            timerManager = CreateTimerManager();
            notifications = CreateNotifications(timerManager);
            sharedStorage = CreateSharedStorage("storage", timerManager);

            inputManager = new DemoPlayerInputManager();
            assetManager = CreateAssetManager();

            rootController = CreateRootController();
            AddGameObject(rootController);

            started = true;
            OnStart();

            inputManager.AddInputListener(rootController);
            rootController.Start();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public void Update(float delta)
        {
            if (m_mode == Mode.Demo)
            {
                m_demoPlayer.ReadTick();
            }
            else
            {
                RunUpdate(delta);
            }
        }

        public void Draw(GraphicsDevice graphicsDevice)
        {
            context.Begin(graphicsDevice);
            drawables.Draw(context);
            OnDrawDebug(context);
            context.End();
        }

        public void RunUpdate(float delta)
        {
            m_currentTime += delta;
            m_frameTime = delta;

            updatables.Update(delta);
            timerManager.Update(delta);
            OnUpdateDebug(delta);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        protected virtual void OnUpdateDebug(float delta)
        {
        }

        [System.Diagnostics.Conditional("DEBUG")]
        protected virtual void OnDrawDebug(Context context)
        {
        }

        //////////////////////////////////////////////////////////////////////////////

        public static Timer ScheduleTimer(TimerCallback callback, float delay = 0.0f, bool repeated = false)
        {
            return ScheduleTimer(callback, delay, repeated ? 0 : 1);
        }

        public static Timer ScheduleTimerOnce(TimerCallback callback, float delay = 0.0f, bool repeated = false)
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

        #region Debug demo

        [System.Diagnostics.Conditional("DEBUG_DEMO")]
        protected void CreateDemoRecorder()
        {
            m_demoRecorder = new DemoRecorder();
            AddUpdatable(m_demoRecorder);
        }

        [System.Diagnostics.Conditional("DEBUG_DEMO")]
        protected void InitDemoRecorder()
        {
            inputManager.AddInputListener(m_demoRecorder);
        }

        [System.Diagnostics.Conditional("DEBUG_DEMO")]
        protected void SaveDemo()
        {
            if (m_demoRecorder != null)
            {
                String path = "demo.dm";
                m_demoRecorder.Save(path);
                Log.d("Demo saved: " + path);
            }
        }

        #endregion

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

        public static SharedStorage Storage()
        {
            return sharedApplication.sharedStorage;
        }

        public static NotificationCenter NotificationCenter()
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

        public static float CurrentTime
        {
            get { return sharedApplication.m_currentTime; }
        }

        public static float frameTime
        {
            get { return sharedApplication.m_frameTime; }
        }

        #endregion
    }
}
