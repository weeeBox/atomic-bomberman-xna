﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual;

namespace BomberEngine.Game
{
    public class ScreenManager : BaseElement
    {   
        private List<Screen> scenes;
        private UpdatableList updatables;
        private DrawableList drawables;

        private Screen currentScene;

        public ScreenManager()
        {
            scenes = new List<Screen>();
            updatables = new UpdatableList();
            drawables = new DrawableList();
        }
        
        //////////////////////////////////////////////////////////////////////////////

        #region Updatable

        public override void Update(float delta)
        {
            updatables.Update(delta);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Drawable

        public override void Draw(Context context)
        {
            drawables.Draw(context);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region InputListener methods

        public override void OnKeyPressed(Keys key)
        {
            currentScene.OnKeyPressed(key);
        }

        public override void OnKeyReleased(Keys key)
        {
            currentScene.OnKeyReleased(key);
        }

        public override void OnButtonPressed(ButtonEvent e)
        {
            currentScene.OnButtonPressed(e);
        }

        public override void OnButtonReleased(ButtonEvent e)
        {
            currentScene.OnButtonReleased(e);
        }

        public override void GamePadConnected(int playerIndex)
        {
            currentScene.GamePadConnected(playerIndex);
        }

        public override void GamePadDisconnected(int playerIndex)
        {
            currentScene.GamePadDisconnected(playerIndex);
        }

        public override void OnPointerMoved(int x, int y, int fingerId)
        {
            currentScene.OnPointerMoved(x, y, fingerId);
        }

        public override void OnPointerPressed(int x, int y, int fingerId)
        {
            currentScene.OnPointerPressed(x, y, fingerId);
        }

        public override void OnPointerDragged(int x, int y, int fingerId)
        {
            currentScene.OnPointerDragged(x, y, fingerId);
        }

        public override void OnPointerReleased(int x, int y, int fingerId)
        {
            currentScene.OnPointerReleased(x, y, fingerId);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Scene Container

        public void StartScene(Screen scene)
        {
            StartScene(scene, false);
        }

        public void StartScene(Screen scene, bool replaceCurrent)
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
                    currentScene.screenManager = null;
                }
                else
                {
                    if (!scene.allowsUpdatePrevious)
                    {
                        currentScene.Suspend();
                        updatables.Remove(currentScene);
                    }

                    if (!scene.allowsDrawPrevious)
                    {   
                        drawables.Remove(currentScene);
                    }
                }
            }

            currentScene = scene;

            scenes.Add(scene);
            scene.screenManager = this;
            scene.Start();

            updatables.Add(scene);
            drawables.Add(scene);
        }

        public void RemoveScene(Screen scene)
        {
            if (scene.screenManager != this)
            {
                throw new InvalidOperationException("Scene doesn't belong to this container: " + scene);
            }

            if (!scenes.Contains(scene))
            {
                throw new InvalidOperationException("Scene manager doesn't contain the scene: " + scene);   
            }

            scene.screenManager = null;
            
            scenes.Remove(scene);
            updatables.Remove(scene);
            drawables.Remove(scene);

            if (scene == currentScene)
            {
                if (scenes.Count > 0)
                {
                    currentScene = scenes[scenes.Count - 1];
                    if (!scene.allowsDrawPrevious)
                    {
                        drawables.Add(currentScene);
                    }

                    if (!scene.allowsUpdatePrevious)
                    {
                        currentScene.Resume();
                        updatables.Add(currentScene);
                    }
                }
                else
                {
                    currentScene = null;
                }
            }
        }

        public Screen CurrentScene()
        {
            return currentScene;
        }

        public bool IsCurrent(Screen scene)
        {
            return CurrentScene() == scene;
        }

        #endregion
    }
}