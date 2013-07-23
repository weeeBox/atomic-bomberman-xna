using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Events;

namespace BomberEngine.Game
{
    public interface IScreenManagerListener
    {
        void OnScreenStarted(ScreenManager manager, Screen screen);
        void OnScreenSuspended(ScreenManager manager, Screen screen);
        void OnScreenResumed(ScreenManager manager, Screen screen);
        void OnScreenStopped(ScreenManager manager, Screen screen);
    }

    public class ScreenManager : BaseElement, IDestroyable
    {
        public IScreenManagerListener listener;

        private List<Screen> screens;

        private UpdatableList updatables;
        private DrawableList drawables;

        private Screen currentScreen;

        public ScreenManager()
        {
            screens = new List<Screen>();
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

        #region Event handlers

        public override bool HandleEvent(Event evt)
        {
            return currentScreen.HandleEvent(evt);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Screen Container

        public void StartPopup(Popup popup)
        {
            StartScreen(popup, false);
        }

        public void StartScreen(Screen screen)
        {
            StartScreen(screen, false);
        }

        public void StartScreen(Screen screen, bool replaceCurrent)
        {
            if (screens.Contains(screen))
            {
                throw new InvalidOperationException("Screen already started: " + screen);
            }

            if (currentScreen != null)
            {
                if (replaceCurrent)
                {
                    currentScreen.screenManager = null;
                    updatables.Remove(currentScreen);
                    drawables.Remove(currentScreen);
                    screens.Remove(currentScreen);

                    Stop(currentScreen);
                }
                else
                {
                    if (!screen.AllowsUpdatePrevious)
                    {
                        Suspend(currentScreen);
                        updatables.Remove(currentScreen);
                    }

                    if (!screen.AllowsDrawPrevious)
                    {   
                        drawables.Remove(currentScreen);
                    }
                }
            }

            currentScreen = screen;

            screens.Add(screen);
            screen.screenManager = this;

            Start(screen);

            updatables.Add(screen);
            drawables.Add(screen);
        }

        public void RemoveScreen(Screen screen)
        {
            if (screen.screenManager != this)
            {
                throw new InvalidOperationException("Screen doesn't belong to this container: " + screen);
            }

            if (!screens.Contains(screen))
            {
                throw new InvalidOperationException("Screen manager doesn't contain the screen: " + screen);   
            }

            screens.Remove(screen);
            updatables.Remove(screen);
            drawables.Remove(screen);

            if (screen == currentScreen)
            {
                if (screens.Count > 0)
                {
                    currentScreen = screens[screens.Count - 1];
                    if (!screen.AllowsDrawPrevious)
                    {
                        drawables.Add(currentScreen);
                    }

                    if (!screen.AllowsUpdatePrevious)
                    {
                        Resume(currentScreen);
                        updatables.Add(currentScreen);
                    }
                }
                else
                {
                    currentScreen = null;
                }
            }

            screen.screenManager = null;
            Stop(screen);
        }

        private void Start(Screen screen)
        {
            screen.Start();
            if (listener != null)
            {
                listener.OnScreenStarted(this, screen);
            }
        }

        private void Suspend(Screen screen)
        {
            screen.Suspend();
            if (listener != null)
            {
                listener.OnScreenSuspended(this, screen);
            }
        }

        private void Resume(Screen screen)
        {
            screen.Resume();
            if (listener != null)
            {
                listener.OnScreenResumed(this, screen);
            }
        }

        private void Stop(Screen screen)
        {
            screen.Stop();
            if (listener != null)
            {
                listener.OnScreenStopped(this, screen);
            }
        }

        public Screen CurrentScreen()
        {
            return currentScreen;
        }

        public bool IsCurrent(Screen screen)
        {
            return CurrentScreen() == screen;
        }

        public Screen FindScreen(int id)
        {
            for (int i = screens.Count-1; i >= 0; --i)
            {
                Screen screen = screens[i];
                if (screen.id == id)
                {
                    return screen;
                }
            }

            return null;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Destroyable

        public void Destroy()
        {
            updatables.Destroy();
        }

        #endregion
    }
}
