using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual;
using BomberEngine.Debugging;
using Microsoft.Xna.Framework.Input;

namespace BomberEngine.Game
{
    public abstract class RootController : BaseElement
    {   
        private Controller currentController;
        private GameConsole console;

        public RootController()
        {   
        }

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

        public override void Update(float delta)
        {
            currentController.Update(delta);
        }

        public override void Draw(Context context)
        {
            currentController.Draw(context);
        }

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

                currentController.Stop();
            }

            currentController = controller;
            controller.Start();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Console

        protected abstract GameConsole CreateConsole();

        protected void ToggleConsole()
        {
            if (console != null)
            {
                if (currentController.IsCurrentScene(console))
                {
                    console.Finish();
                }
                else
                {
                    currentController.StartNextScene(console);
                }
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region InputListener

        public override void KeyPressed(Keys key)
        {
            currentController.KeyPressed(key);
        }

        public override void KeyReleased(Keys key)
        {
            currentController.KeyReleased(key);
        }

        public override void ButtonPressed(ButtonEvent e)
        {
            currentController.ButtonPressed(e);
        }

        public override void ButtonReleased(ButtonEvent e)
        {
            currentController.ButtonReleased(e);
        }

        public override void GamePadConnected(int playerIndex)
        {
            currentController.GamePadConnected(playerIndex);
        }

        public override void GamePadDisconnected(int playerIndex)
        {
            currentController.GamePadDisconnected(playerIndex);
        }

        public override void PointerMoved(int x, int y, int fingerId)
        {
            currentController.PointerMoved(x, y, fingerId);
        }

        public override void PointerPressed(int x, int y, int fingerId)
        {
            currentController.PointerPressed(x, y, fingerId);
        }

        public override void PointerDragged(int x, int y, int fingerId)
        {
            currentController.PointerDragged(x, y, fingerId);
        }

        public override void PointerReleased(int x, int y, int fingerId)
        {
            currentController.PointerReleased(x, y, fingerId);
        }

        #endregion
    }
}