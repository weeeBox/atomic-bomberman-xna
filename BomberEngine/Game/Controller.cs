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
        }

        public void Stop()
        {   
        }

        public void Update(float delta)
        {
            sceneManager.Update(delta);
        }

        public void Draw(Context context)
        {
            sceneManager.Draw(context);
        }

        public InputListener InputListener
        {
            get { return sceneManager; }
        }
    }
}
