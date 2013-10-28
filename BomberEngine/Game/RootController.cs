using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BomberEngine
{
    public enum KeyCatch
    {
        None    = 0x0,
        Console = 0x01,
    }

    public abstract class RootController : BaseElement, IInputListener
    {
        private ContentManager m_contentManager;

        private Controller m_currentController;
        private CConsole m_console;
        private View m_debugView;

        private CKeyBindings m_keyBindings;
        private KeyCatch m_keyCatch;

        public RootController(ContentManager contentManager)
        {
            this.m_contentManager = contentManager;
            m_keyBindings = new CKeyBindings();
            m_keyCatch = KeyCatch.None;

            InitDebugView();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        public void Start()
        {
            m_console = CreateConsole();
            m_console.TryExecuteCommand("exec default.cfg");

            OnStart();
        }

        public void Stop()
        {
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

        #region Updatable

        public override void Update(float delta)
        {
            m_currentController.Update(delta);
            UpdateDebugView(delta);
            UpdateConsole(delta);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Drawable

        public override void Draw(Context context)
        {
            m_currentController.Draw(context);
            DrawDebugView(context);
            DrawConsole(context);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Child controllers

        public void StartController(Controller controller)
        {
            if (controller == null)
            {
                throw new ArgumentException("Controller is null");
            }

            if (m_currentController != null)
            {
                if (controller == m_currentController)
                {
                    throw new InvalidOperationException("Controller already set as current: " + controller);
                }

                if (controller.ParentController == m_currentController)
                {
                    m_currentController.Suspend();
                }
                else
                {
                    m_currentController.Stop();
                }
                
            }

            m_currentController = controller;
            m_currentController.Start();
        }

        internal void ControllerStopped(Controller controller)
        {
            Debug.Assert(controller == m_currentController);
            m_currentController = null;

            OnControllerStop(controller);
        }

        protected virtual void OnControllerStop(Controller controller)
        {

        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Console

        protected virtual CConsole CreateConsole()
        {
            Font consoleFont = new VectorFont(m_contentManager.Load<SpriteFont>("ConsoleFont"));
            CConsole console = new CConsole(consoleFont);

            console.RegisterCommand(new Cmd_exit());
            console.RegisterCommand(new Cmd_listcmds());
            console.RegisterCommand(new Cmd_listcvars());
            console.RegisterCommand(new Cmd_exec());
            console.RegisterCommand(new Cmd_write());
            console.RegisterCommand(new Cmd_bind());
            console.RegisterCommand(new Cmd_unbind());
            console.RegisterCommand(new Cmd_unbind_all());
            console.RegisterCommand(new Cmd_bindlist());
            console.RegisterCommand(new Cmd_ctoggle());

            console.RegisterCvar(CVars.g_drawViewBorders);
            console.RegisterCvar(CVars.d_demoTargetFrame);

            return console;
        }

        #if NO_CONSOLE
        [System.Diagnostics.Conditional("FALSE")]
        #endif
        private void UpdateConsole(float delta)
        {
            if (m_console.IsVisible)
            {
                m_console.Update(delta);
            }
        }

        #if NO_CONSOLE
        [System.Diagnostics.Conditional("FALSE")]
        #endif
        private void DrawConsole(Context context)
        {
            if (m_console.IsVisible)
            {
                m_console.Draw(context);
            }
        }

        #if NO_CONSOLE
        [System.Diagnostics.Conditional("FALSE")]
        #endif
        protected void ToggleConsole()
        {
            m_console.ToggleVisible();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Event handler

        public override bool HandleEvent(Event evt)
        {
            if (m_console.IsVisible && m_console.HandleEvent(evt))
            {
                return true;
            }

            return m_currentController.HandleEvent(evt);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Events

        private KeyEvent keyEvent = new KeyEvent();

        internal void SetKeyCatch(KeyCatch keyCatch)
        {
            m_keyCatch |= keyCatch;
        }

        internal void RemoveKeyCatch(KeyCatch keyCatch)
        {
            m_keyCatch &= ~keyCatch;
        }

        internal bool HasKeyCatch(KeyCatch keyCatch)
        {
            return (m_keyCatch & keyCatch) != 0;
        }

        internal bool HasKeyCatch()
        {
            return m_keyCatch != KeyCatch.None;
        }

        internal void ClearKeyCatch()
        {
            m_keyCatch = KeyCatch.None;
        }
        
        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Input listener

        public virtual bool OnKeyPressed(KeyEventArg arg)
        {
            if (!HasKeyCatch())
            {
                m_keyBindings.OnKeyPressed(arg);
            }
            
            return HandleEvent(keyEvent.Init(arg, KeyState.Pressed));
        }

        public virtual bool OnKeyRepeated(KeyEventArg arg)
        {
            return HandleEvent(keyEvent.Init(arg, KeyState.Repeated));
        }

        public virtual bool OnKeyReleased(KeyEventArg arg)
        {
            if (!HasKeyCatch())
            {
                m_keyBindings.OnKeyReleased(arg);
            }
            
            return HandleEvent(keyEvent.Init(arg, KeyState.Released));
        }

        public virtual void OnPointerMoved(int x, int y, int fingerId)
        {
            throw new NotImplementedException();
        }

        public virtual void OnPointerPressed(int x, int y, int fingerId)
        {
            throw new NotImplementedException();
        }

        public virtual void OnPointerDragged(int x, int y, int fingerId)
        {
            throw new NotImplementedException();
        }

        public virtual void OnPointerReleased(int x, int y, int fingerId)
        {
            throw new NotImplementedException();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public CConsole Console
        {
            get { return m_console; }
        }

        public Controller GetCurrentController()
        {
            return m_currentController;
        }

        public CKeyBindings GetKeyBindings()
        {
            return m_keyBindings;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Debug

        [System.Diagnostics.Conditional("DEBUG_VIEW")]
        private void InitDebugView()
        {
            int appWidth = Application.GetWidth();
            int appHeight = Application.GetHeight();
            int realWidth = Application.GetRealWidth();
            int realHeight = Application.GetRealHeight();

            int width = realWidth - appWidth;
            int height = realHeight;

            m_debugView = new View(appWidth, 0, width, height);
        }

        [System.Diagnostics.Conditional("DEBUG_VIEW")]
        private void UpdateDebugView(float delta)
        {
            m_debugView.Update(delta);
        }

        [System.Diagnostics.Conditional("DEBUG_VIEW")]
        private void DrawDebugView(Context context)
        {
            m_debugView.Draw(context);
        }

        [System.Diagnostics.Conditional("DEBUG_VIEW")]
        public void AddDebugView(View view)
        {
            m_debugView.AddView(view);
            m_debugView.LayoutVer(0);
        }

        [System.Diagnostics.Conditional("DEBUG_VIEW")]
        public void RemoveDebugView(View view)
        {
            m_debugView.RemoveView(view);
            m_debugView.LayoutVer(0);
        }

        #endregion
    }
}