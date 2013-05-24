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
    public class ScreenManager : BaseElement
    {   
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
                    currentScreen.Stop();
                    currentScreen.screenManager = null;
                }
                else
                {
                    if (!screen.AllowsUpdatePrevious)
                    {
                        currentScreen.Suspend();
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
            screen.Start();

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

            screen.screenManager = null;
            
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
                        currentScreen.Resume();
                        updatables.Add(currentScreen);
                    }
                }
                else
                {
                    currentScreen = null;
                }
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

        #endregion
    }
}
