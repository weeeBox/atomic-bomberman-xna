using System;
using Microsoft.Xna.Framework.Graphics;

namespace BomberEngine
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
        protected enum Mode
        {
            Normal,
            Demo,
        }

        public static Application sharedApplication;

        private RootController m_rootController;

        protected Context m_context;

        private bool m_started;
        private bool m_stoped;

        private CommandLine m_cmdLine;

        private TimerManager m_timerManager;
        private InputManager m_inputManager;
        private AssetManager m_assetManager;
        private NotificationCenter m_notifications;
        private SharedStorage m_sharedStorage;

        private UpdatableList m_updatables;
        private DrawableList m_drawables;

        private INativeInterface m_nativeInterface;
        private int m_width;
        private int m_height;
        private int m_realWidth;
        private int m_realHeight;

        private float m_currentTime;
        private float m_frameTime;
        private long m_tickIndex;

        private Mode m_mode;
        private DemoCmdEntry m_demoCmdEntry;

        private DemoPlayer m_demoPlayer;
        private DemoRecorder m_demoRecorder;

        public Application(ApplicationInfo info)
        {
            m_nativeInterface = info.nativeInterface;
            m_width = info.width;
            m_height = info.height;
            m_realWidth = info.realWidth;
            m_realHeight = info.realHeight;

            sharedApplication = this;

            m_context = new Context();
            m_updatables = new UpdatableList(1);
            m_drawables = new DrawableList(1);

            m_cmdLine = CreateCommandLine();
            m_cmdLine.Parse(info.args);
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
            if (m_started)
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
            m_stoped = true;
        }

        public void RunStop()
        {
            if (!m_started)
            {
                throw new InvalidOperationException("Application not started");
            }

            m_rootController.Stop();
            OnStop();

            m_sharedStorage.Destroy();
            m_timerManager.Destroy();
            m_inputManager.Destroy();

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

            m_timerManager = CreateTimerManager();
            m_notifications = CreateNotifications(m_timerManager);
            m_sharedStorage = CreateSharedStorage("storage", m_timerManager);

            m_inputManager = CreateInputManager();
            if (m_inputManager is IUpdatable)
            {
                AddUpdatable(m_inputManager as IUpdatable);
            }

            CreateDemoRecorder();
            
            m_assetManager = CreateAssetManager();

            m_rootController = CreateRootController();
            AddGameObject(m_rootController);

            m_started = true;
            OnStart();

            m_inputManager.AddInputListener(m_rootController);
            m_rootController.Start();
        }

        private void StartDemo(String path)
        {
            m_mode = Mode.Demo;

            m_timerManager = CreateTimerManager();
            m_notifications = CreateNotifications(m_timerManager);
            m_sharedStorage = CreateSharedStorage("storage", m_timerManager);

            m_demoPlayer = new DemoPlayer(path);

            m_inputManager = new DemoPlayerInputManager();
            m_assetManager = CreateAssetManager();

            m_rootController = CreateRootController();
            AddGameObject(m_rootController);

            m_started = true;
            OnStart();

            m_inputManager.AddInputListener(m_rootController);
            m_rootController.Start();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        public void Update(float delta)
        {
            if (m_mode == Mode.Demo)
            {
                m_demoPlayer.Update(delta);
            }
            else
            {
                if (Debug.IsDebuggerAttached())
                {
                    RunUpdate(delta);
                }
                else
                {
                    try
                    {
                        RunUpdate(delta);
                    }
                    catch (Exception e)
                    {
                        RunCrash(e);
                        throw e;
                    }
                }
            }
        }

        public void Draw(GraphicsDevice graphicsDevice)
        {
            m_context.Begin(graphicsDevice);
            m_drawables.Draw(m_context);
            OnDrawDebug(m_context);
            m_context.End();
        }

        public void RunUpdate(float delta)
        {
            m_currentTime += delta;
            m_frameTime = delta;

            m_updatables.Update(delta);
            m_timerManager.Update(delta);
            OnUpdateDebug(delta);

            ++m_tickIndex;
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

        public static Timer ScheduleTimer(TimerCallback1 callback, float delay = 0.0f, bool repeated = false)
        {
            return ScheduleTimer(callback, delay, repeated ? 0 : 1);
        }

        public static Timer ScheduleTimer(TimerCallback2 callback, float delay = 0.0f, bool repeated = false)
        {
            return ScheduleTimer(callback, delay, repeated ? 0 : 1);
        }

        public static Timer ScheduleTimerOnce(TimerCallback1 callback, float delay = 0.0f, bool repeated = false)
        {
            return ScheduleTimerOnce(callback, delay, repeated ? 0 : 1);
        }

        public static Timer ScheduleTimerOnce(TimerCallback2 callback, float delay = 0.0f, bool repeated = false)
        {
            return ScheduleTimerOnce(callback, delay, repeated ? 0 : 1);
        }

        public static Timer ScheduleTimer(TimerCallback1 callback, float delay, int numRepeats)
        {
            return sharedApplication.m_timerManager.Schedule(callback, delay, numRepeats);
        }

        public static Timer ScheduleTimer(TimerCallback2 callback, float delay, int numRepeats)
        {
            return sharedApplication.m_timerManager.Schedule(callback, delay, numRepeats);
        }

        public static Timer ScheduleTimerOnce(TimerCallback1 callback, float delay, int numRepeats)
        {
            return sharedApplication.m_timerManager.ScheduleOnce(callback, delay, numRepeats);
        }

        public static Timer ScheduleTimerOnce(TimerCallback2 callback, float delay, int numRepeats)
        {
            return sharedApplication.m_timerManager.ScheduleOnce(callback, delay, numRepeats);
        }

        public static void CancelTimer(TimerCallback1 callback)
        {
            sharedApplication.m_timerManager.Cancel(callback);
        }

        public static void CancelTimer(TimerCallback2 callback)
        {
            sharedApplication.m_timerManager.Cancel(callback);
        }

        public static void CancelAllTimers(Object target)
        {
            sharedApplication.m_timerManager.CancelAll(target);
        }

        public static void CancelAllTimers()
        {
            sharedApplication.m_timerManager.CancelAll();
        }

        //////////////////////////////////////////////////////////////////////////////
        protected void AddUpdatable(IUpdatable updatable)
        {
            m_updatables.Add(updatable);
        }

        protected void RemoveUpdatable(IUpdatable updatable)
        {
            m_updatables.Remove(updatable);
        }

        protected void AddDrawable(IDrawable drawable)
        {
            m_drawables.Add(drawable);
        }

        protected void RemoveDrawable(IDrawable drawable)
        {
            m_drawables.Remove(drawable);
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

            m_inputManager.AddInputListener(m_demoRecorder);
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
            sharedApplication.m_nativeInterface.SetWindowTitle(title);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Getters/Setters

        public static AssetManager Assets()
        {
            return sharedApplication.m_assetManager;
        }

        public static RootController RootController()
        {
            return sharedApplication.m_rootController;
        }

        public static TimerManager TimerManager()
        {
            return sharedApplication.m_timerManager;
        }

        public static SharedStorage Storage()
        {
            return sharedApplication.m_sharedStorage;
        }

        public static NotificationCenter NotificationCenter()
        {
            return sharedApplication.m_notifications;
        }

        public bool IsRunning()
        {
            return !m_stoped;
        }

        public static int GetWidth()
        {
            return sharedApplication.m_width;
        }

        public static int GetHeight()
        {
            return sharedApplication.m_height;
        }

        public static int GetRealWidth()
        {
            return sharedApplication.m_realWidth;
        }

        public static int GetRealHeight()
        {
            return sharedApplication.m_realWidth;
        }

        public static float CurrentTime
        {
            get { return sharedApplication.m_currentTime; }
        }

        public static float frameTime
        {
            get { return sharedApplication.m_frameTime; }

            #if UNIT_TESTING
            set { sharedApplication.m_frameTime = value; }
            #endif
        }

        public static long tickIndex
        {
            get { return sharedApplication.m_tickIndex; }
        }

        protected Mode mode
        {
            get { return m_mode; }
        }

        #if UNIT_TESTING

        

        #endif

        #endregion
    }
}
