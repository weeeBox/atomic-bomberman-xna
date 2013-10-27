using BomberEngine;
using BomberEngine.UI;
using Bomberman.Menu;
using Microsoft.Xna.Framework;

namespace Bomberman.Game.Screens
{
    public class PauseScreen : Screen
    {
        public enum ButtonId
        {
            Resume,
            Exit,
        }

        public PauseScreen(ButtonDelegate buttonDelegate)
        {   
            AllowsDrawPrevious = true;

            Color backColor = new Color(0.0f, 0.0f, 0.0f, 0.25f);
            AddView(new RectView(0, 0, width, height, backColor, Color.Black));

            ButtonGroup group = new ButtonGroup();
            Button resumeButton = group.AddButton("Resume", (int)ButtonId.Resume, buttonDelegate);
            group.AddButton("Exit", (int)ButtonId.Exit, buttonDelegate);
            group.alignX = group.alignY = View.ALIGN_CENTER;
            group.x = 0.5f * width;
            group.y = 0.5f * height;
            AddView(group);

            SetCancelButton(resumeButton);
        }
    }
}
