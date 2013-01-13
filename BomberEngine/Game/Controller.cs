using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Input;

namespace BomberEngine.Game
{
    public class Controller : Updatable, Drawable
    {
        private ScenesManager sceneManager;

        public Controller()
        {
            sceneManager = new ScenesManager();
        }

        public void Start()
        {
            OnStart();
        }

        public void Stop()
        {
            OnStop();
        }

        public void Update(float delta)
        {
            sceneManager.Update(delta);
        }

        public void Draw(Context context)
        {
            sceneManager.Draw(context);
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnStop()
        {
        }

        protected void StartScene(Scene scene)
        {
            sceneManager.StartScene(scene, true);
        }

        protected void StartNextScene(Scene scene)
        {
            sceneManager.StartScene(scene, false);
        }

        public InputListener InputListener
        {
            get { return sceneManager; }
        }
    }
}