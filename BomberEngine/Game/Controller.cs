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
        private SceneManager sceneManager;

        public Controller()
        {
            sceneManager = new SceneManager();
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
            sceneManager.Update(delta);
        }

        public override void Draw(Context context)
        {
            sceneManager.Draw(context);
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnStop()
        {
        }

        public Scene CurrentScene()
        {
            return sceneManager.CurrentScene();
        }

        public bool IsCurrentScene(Scene scene)
        {
            return sceneManager.IsCurrent(scene);
        }

        public void StartScene(Scene scene)
        {
            sceneManager.StartScene(scene, true);
        }

        public void StartNextScene(Scene scene)
        {
            sceneManager.StartScene(scene, false);
        }

        public override void OnKeyPressed(Keys key)
        {
            sceneManager.OnKeyPressed(key);
        }

        public override void OnKeyReleased(Keys key)
        {
            sceneManager.OnKeyReleased(key);
        }

        public override void OnButtonPressed(ButtonEvent e)
        {
            sceneManager.OnButtonPressed(e);
        }

        public override void OnButtonReleased(ButtonEvent e)
        {
            sceneManager.OnButtonReleased(e);
        }

        public override void GamePadConnected(int playerIndex)
        {
            sceneManager.GamePadConnected(playerIndex);
        }

        public override void GamePadDisconnected(int playerIndex)
        {
            sceneManager.GamePadDisconnected(playerIndex);
        }

        public override void PointerMoved(int x, int y, int fingerId)
        {
            sceneManager.PointerMoved(x, y, fingerId);
        }

        public override void PointerPressed(int x, int y, int fingerId)
        {
            sceneManager.PointerPressed(x, y, fingerId);
        }

        public override void PointerDragged(int x, int y, int fingerId)
        {
            sceneManager.PointerDragged(x, y, fingerId);
        }

        public override void PointerReleased(int x, int y, int fingerId)
        {
            sceneManager.PointerReleased(x, y, fingerId);
        }
    }
}