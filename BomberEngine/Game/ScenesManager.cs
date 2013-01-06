using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core;
using BomberEngine.Core.Input;

namespace BomberEngine.Game
{
    public class ScenesManager : ScenesContainer, Updatable, Drawable, InputListener
    {   
        private List<Scene> scenes;
        private Scene currentScene;

        public ScenesManager()
        {
            scenes = new List<Scene>();
        }
        
        //////////////////////////////////////////////////////////////////////////////

        #region Updatable

        public void Update(float delta)
        {   
        }

        protected void AddUpdatabled(Updatable updatable)
        {   
        }

        protected void RemoveUpdatable(Updatable updatable)
        {   
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Drawable

        public void Draw(Context context)
        {
   
        }

        protected void AddDrawable(Drawable drawable)
        {   
        }

        protected void RemoveDrawable(Drawable drawable)
        {   
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region InputListener methods

        public void KeyPressed(Keys key)
        {
        }

        public void KeyReleased(Keys key)
        {
        }

        public void ButtonPressed(ButtonEvent e)
        {
        }

        public void ButtonReleased(ButtonEvent e)
        {
        }

        public void GamePadConnected(int playerIndex)
        {
        }

        public void GamePadDisconnected(int playerIndex)
        {
        }

        public void PointerMoved(int x, int y, int fingerId)
        {
        }

        public void PointerPressed(int x, int y, int fingerId)
        {
        }

        public void PointerDragged(int x, int y, int fingerId)
        {
        }

        public void PointerReleased(int x, int y, int fingerId)
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Scene Container

        public void StartScene(Scene scene)
        {
            StartScene(scene, false);
        }

        public void StartScene(Scene scene, bool replaceCurrent)
        {
            if (scenes.Contains(scene))
            {
                throw new InvalidOperationException("Scene already started: " + scene);
            }

            if (currentScene != null)
            {
                if (replaceCurrent)
                {
                    currentScene.Stop();
                    currentScene.SceneContainer = null;
                }
                else
                {
                    currentScene.PushBack();
                }
            }

            scenes.Add(scene);
            scene.SceneContainer = this;
            scene.Start();
        }

        public void RemoveScene(Scene scene)
        {
            if (scene.SceneContainer != this)
            {
                throw new InvalidOperationException("Scene doesn't belong to this container: " + scene);
            }

            if (!scenes.Contains(scene))
            {
                throw new InvalidOperationException("Scene manager doesn't contain the scene: " + scene);   
            }

            scene.SceneContainer = null;
            scenes.Remove(scene);

            if (scene == currentScene && scenes.Count > 0)
            {
                currentScene = scenes[scenes.Count - 1];
                currentScene.BringFront();
            }
        }

        #endregion
    }
}
