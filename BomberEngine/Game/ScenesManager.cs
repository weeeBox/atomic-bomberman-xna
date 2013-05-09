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
    public class ScenesManager : View
    {   
        private List<Scene> scenes;
        private Scene currentScene;

        public ScenesManager()
        {
            scenes = new List<Scene>();
        }
        
        //////////////////////////////////////////////////////////////////////////////

        #region InputListener methods

        public override void KeyPressed(Keys key)
        {
            currentScene.KeyPressed(key);
        }

        public override void KeyReleased(Keys key)
        {
            currentScene.KeyReleased(key);
        }

        public override void ButtonPressed(ButtonEvent e)
        {
            currentScene.ButtonPressed(e);
        }

        public override void ButtonReleased(ButtonEvent e)
        {
            currentScene.ButtonReleased(e);
        }

        public override void GamePadConnected(int playerIndex)
        {
            currentScene.GamePadConnected(playerIndex);
        }

        public override void GamePadDisconnected(int playerIndex)
        {
            currentScene.GamePadDisconnected(playerIndex);
        }

        public override void PointerMoved(int x, int y, int fingerId)
        {
            currentScene.PointerMoved(x, y, fingerId);
        }

        public override void PointerPressed(int x, int y, int fingerId)
        {
            currentScene.PointerPressed(x, y, fingerId);
        }

        public override void PointerDragged(int x, int y, int fingerId)
        {
            currentScene.PointerDragged(x, y, fingerId);
        }

        public override void PointerReleased(int x, int y, int fingerId)
        {
            currentScene.PointerReleased(x, y, fingerId);
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
                    currentScene.sceneManager = null;
                }
                else
                {
                    if (!scene.allowsUpdatePrevious)
                    {
                        currentScene.Suspend();
                        RemoveUpdatable(currentScene);
                    }

                    if (!scene.allowsDrawPrevious)
                    {   
                        RemoveDrawable(currentScene);
                    }
                }
            }

            currentScene = scene;

            scenes.Add(scene);
            scene.sceneManager = this;
            scene.Start();

            Add(scene);
        }

        public void RemoveScene(Scene scene)
        {
            if (scene.sceneManager != this)
            {
                throw new InvalidOperationException("Scene doesn't belong to this container: " + scene);
            }

            if (!scenes.Contains(scene))
            {
                throw new InvalidOperationException("Scene manager doesn't contain the scene: " + scene);   
            }

            scene.sceneManager = null;
            scenes.Remove(scene);

            Remove(scene);

            if (scene == currentScene)
            {
                if (scenes.Count > 0)
                {
                    currentScene = scenes[scenes.Count - 1];
                    if (!scene.allowsDrawPrevious)
                    {
                        AddDrawable(currentScene);
                    }

                    if (!scene.allowsUpdatePrevious)
                    {
                        currentScene.Resume();
                        AddUpdatable(currentScene);
                    }
                }
                else
                {
                    currentScene = null;
                }
            }
        }

        public Scene CurrentScene()
        {
            return currentScene;
        }

        public bool IsCurrent(Scene scene)
        {
            return CurrentScene() == scene;
        }

        #endregion
    }
}
