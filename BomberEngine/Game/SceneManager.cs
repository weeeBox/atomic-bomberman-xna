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
    public class SceneManager : SceneListener, Updatable, Drawable, InputListener
    {   
        private List<Scene> scenes;
        private Scene currentScene;

        public SceneManager()
        {
            scenes = new List<Scene>();
        }

        //////////////////////////////////////////////////////////////////////////////

        public void StartScene(Scene scene)
        {
            if (ContainsScene(scene))
            {
                throw new InvalidOperationException("Scene already started: " + scenes);
            }

            if (currentScene != null)
            {
                currentScene.Stop();
                currentScene.Listener = null;
            }

            scene.Listener = this;
            scene.Start();
        }

        private void AddScene(Scene scene)
        {

        }

        private void RemoveScene(Scene scene)
        {

        }

        private bool ContainsScene(Scene scene)
        {
            return scenes.Contains(scene);
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

        #region Scene Listener

        public void OnSceneStarted(Scene scene)
        {
            AddScene(scene);
        }

        public void OnSceneStoped(Scene scene)
        {
            RemoveScene(scene);
        }

        #endregion
    }
}
