using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual;
using BomberEngine.Debugging;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core.Events;
using BomberEngine.Consoles;
using BomberEngine.Core.Assets.Types;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BomberEngine.Game
{
    public abstract class RootController : BaseElement, IInputListener
    {
        protected ContentManager contentManager;
        protected Controller currentController;
        protected Cmd console;

        public RootController(ContentManager contentManager)
        {
            this.contentManager = contentManager;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        public void Start()
        {
            console = CreateConsole();
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
            currentController.Update(delta);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Drawable

        public override void Draw(Context context)
        {
            currentController.Draw(context);
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

            if (currentController != null)
            {
                if (controller == currentController)
                {
                    throw new InvalidOperationException("Controller already set as current: " + controller);
                }

                if (controller.ParentController == currentController)
                {
                    currentController.Suspend();
                }
                else
                {
                    currentController.Stop();
                }
                
            }

            currentController = controller;
            currentController.Start();
        }

        internal void ControllerStopped(Controller controller)
        {
            Debug.Assert(controller == currentController);
            currentController = null;

            OnControllerStop(controller);
        }

        protected virtual void OnControllerStop(Controller controller)
        {

        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Console

        protected virtual Cmd CreateConsole()
        {
            Font consoleFont = new VectorFont(contentManager.Load<SpriteFont>("ConsoleFont"));
            Cmd console = new Cmd(consoleFont);

            return console;
        }

        protected void ToggleConsole()
        {
            if (console != null)
            {
                if (currentController.IsCurrentScreen(console))
                {
                    console.Finish();
                }
                else
                {
                    currentController.StartNextScreen(console);
                }
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Event handler

        public override bool HandleEvent(Event evt)
        {
            return currentController.HandleEvent(evt);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Events

        private KeyEvent keyEvent = new KeyEvent();
        private GamePadEvent gamePadConnectivityEvent = new GamePadEvent();

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Input listener

        public virtual bool OnKeyPressed(KeyEventArg arg)
        {
            return HandleEvent(keyEvent.Init(arg, KeyEvent.PRESSED));
        }

        public virtual bool OnKeyRepeated(KeyEventArg arg)
        {
            return HandleEvent(keyEvent.Init(arg, KeyEvent.REPEATED));
        }

        public virtual bool OnKeyReleased(KeyEventArg arg)
        {
            return HandleEvent(keyEvent.Init(arg, KeyEvent.RELEASED));
        }

        public virtual void OnGamePadConnected(int playerIndex)
        {
            throw new NotImplementedException();
        }

        public virtual void OnGamePadDisconnected(int playerIndex)
        {
            throw new NotImplementedException();
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

        public Controller CurrentController
        {
            get { return currentController; }
        }

        #endregion

    }
}