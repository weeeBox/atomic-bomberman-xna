using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core.Visual.UI;
using Assets;
using BomberEngine.Core.Assets.Types;
using Bomberman.Menu;
using BomberEngine.Core.Input;

namespace Bomberman.Game.Screens
{
    public class PauseScreen : Screen
    {
        public const int BUTTON_RESUME = 1;
        public const int BUTTON_EXIT = 2;

        public PauseScreen(ButtonDelegate buttonDelegate)
        {   
            AllowsDrawPrevious = true;

            Color backColor = new Color(0.0f, 0.0f, 0.0f, 0.25f);
            AddView(new RectView(0, 0, width, height, backColor, Color.Black));

            ButtonGroup group = new ButtonGroup();
            group.AddButton("Resume", BUTTON_RESUME, buttonDelegate);
            group.AddButton("Exit", BUTTON_EXIT, buttonDelegate);
            group.alignX = group.alignY = View.ALIGN_CENTER;
            group.x = 0.5f * width;
            group.y = 0.5f * height;
            AddView(group);
        }
    }
}
