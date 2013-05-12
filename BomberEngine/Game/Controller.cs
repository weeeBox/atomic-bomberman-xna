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

        public override void OnPointerMoved(int x, int y, int fingerId)
        {
            sceneManager.OnPointerMoved(x, y, fingerId);
        }

        public override void OnPointerPressed(int x, int y, int fingerId)
        {
            sceneManager.OnPointerPressed(x, y, fingerId);
        }

        public override void OnPointerDragged(int x, int y, int fingerId)
        {
            sceneManager.OnPointerDragged(x, y, fingerId);
        }

        public override void OnPointerReleased(int x, int y, int fingerId)
        {
            sceneManager.OnPointerReleased(x, y, fingerId);
        }
    }
}