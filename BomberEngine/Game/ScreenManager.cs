using System;
using System.Collections.Generic;

namespace BomberEngine
{
    public class ScreenManager : BaseElement, IDestroyable
    {
        private Controller m_controller;

        private List<Screen> screens;

        private UpdatableList updatables;
        private DrawableList drawables;

        private Screen currentScreen;

        public ScreenManager(Controller controller)
        {
            m_controller = controller;

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

                    currentScreen.Stop();
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
            Screen.current = currentScreen;

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

            Screen.current = currentScreen;
            screen.screenManager = null;
            screen.Stop();

            if (IsEmpty)
            {
                m_controller.OnEmptyScreenStack(this);
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

        public bool IsEmpty
        {
            get { return screens.Count == 0; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Destroyable

        public void Destroy()
        {
            updatables.Destroy();
            foreach (Screen screen in screens)
            {
                screen.Destroy();
            }
            screens.Clear();
        }

        #endregion
    }
}
