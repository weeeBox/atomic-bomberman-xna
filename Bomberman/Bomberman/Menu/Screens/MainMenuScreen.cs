﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual.UI;
using Microsoft.Xna.Framework;

namespace Bomberman.Menu.Screens
{
    public class MainMenuScreen : Screen
    {
        public MainMenuScreen()
        {
            int w = 50;
            int h = 15;

            RectView rect = new FocusableRect(10, 10, 200, 400);
            for (int i = 0; i < 4; ++i)
            {
                rect.AddChild(new FocusableRect(10, 0, w, h));
            }

            RectView container = new RectView(10, 0, 150, 150, Color.LightGray, Color.Black);

            for (int i = 0; i < 4; ++i)
            {
                container.AddChild(new FocusableRect(10, 0, w, h));
            }
            container.LayoutVer(10);
            container.ResizeToFitViewsVer(10);

            rect.AddChild(container);

            RectView container2 = new RectView(10, 0, 150, 150, Color.LightGray, Color.Black);
            for (int i = 0; i < 4; ++i)
            {
                container2.AddChild(new FocusableRect(10, 0, w, h));
            }
            container2.LayoutVer(10);
            container2.ResizeToFitViewsVer(10);

            rect.AddChild(container2);

            for (int i = 0; i < 4; ++i)
            {
                rect.AddChild(new FocusableRect(10, 0, w, h));
            }

            rect.LayoutVer(10);
            rect.ResizeToFitViewsVer(10);

            SetRootView(rect);
        }

        public override bool OnKeyPressed(Microsoft.Xna.Framework.Input.Keys key)
        {
            return base.OnKeyPressed(key);
        }
    }

    class FocusableRect : RectView
    {
        private static Color DEFAULT_BORDER = Color.Black;
        private static Color DEFAULT_FILL = Color.DarkGray;

        private static Color FOCUSED_BORDER = Color.Black;
        private static Color FOCUSED_FILL = Color.LightGray;

        public FocusableRect(int x, int y, int width, int height)
            : base(x, y, width, height, DEFAULT_FILL, DEFAULT_BORDER)
        {
            focusable = true;
        }

        protected override void OnFocusChanged(bool focused)
        {
            if (focused)
            {
                borderColor = FOCUSED_BORDER;
                fillColor = FOCUSED_FILL;
            }
            else
            {
                borderColor = DEFAULT_BORDER;
                fillColor = DEFAULT_FILL;
            }
        }
    }
}