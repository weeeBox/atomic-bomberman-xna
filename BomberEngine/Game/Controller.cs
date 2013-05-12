using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual;
using Microsoft.Xna.Framework.Input;

namespace BomberEngine.Game
{
    public class Controller : BaseElement
    {
        private ScreenManager screenManager;

        public Controller()
        {
            screenManager = new ScreenManager();
        }

        public void Start()
        {
            OnStart();
        }

        public void Stop()
        {
            OnStop();
        }

        public override void Update(float delta)
        {
            screenManager.Update(delta);
        }

        public override void Draw(Context context)
        {
            screenManager.Draw(context);
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnStop()
        {
        }

        public Screen CurrentScreen()
        {
            return screenManager.CurrentScreen();
        }

        public bool IsCurrentScreen(Screen screen)
        {
            return screenManager.IsCurrent(screen);
        }

        public void StartScreen(Screen screen)
        {
            screenManager.StartScreen(screen, true);
        }

        public void StartNextScreen(Screen screen)
        {
            screenManager.StartScreen(screen, false);
        }

        public override bool OnKeyPressed(Keys key)
        {
            return screenManager.OnKeyPressed(key);
        }

        public override bool OnKeyReleased(Keys key)
        {
            return screenManager.OnKeyReleased(key);
        }

        public override bool OnButtonPressed(ButtonEvent e)
        {
            return screenManager.OnButtonPressed(e);
        }

        public override bool OnButtonReleased(ButtonEvent e)
        {
            return screenManager.OnButtonReleased(e);
        }

        public override void GamePadConnected(int playerIndex)
        {
            screenManager.GamePadConnected(playerIndex);
        }

        public override void GamePadDisconnected(int playerIndex)
        {
            screenManager.GamePadDisconnected(playerIndex);
        }

        public override void OnPointerMoved(int x, int y, int fingerId)
        {
            screenManager.OnPointerMoved(x, y, fingerId);
        }

        public override void OnPointerPressed(int x, int y, int fingerId)
        {
            screenManager.OnPointerPressed(x, y, fingerId);
        }

        public override void OnPointerDragged(int x, int y, int fingerId)
        {
            screenManager.OnPointerDragged(x, y, fingerId);
        }

        public override void OnPointerReleased(int x, int y, int fingerId)
        {
            screenManager.OnPointerReleased(x, y, fingerId);
        }
    }
}