using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Assets.Types;
using Bomberman.Game;
using Assets;

namespace Bomberman.Menu
{
    public class ButtonGroup : View
    {
        public ButtonGroup()
            : this(0)
        {
        }

        public ButtonGroup(float width)
            : base(width, 0)
        {   
        }

        public void AddButton(String title, int id, ButtonDelegate buttonDelegate)
        {
            Font font = Helper.fontButton;

            TextButton button = new TextButton(title, font, 0, 0, width, font.FontHeight());
            button.id = id;
            button.buttonDelegate = buttonDelegate;
            AddView(button);

            LayoutVer(10);
            ResizeToFitViewsVer();
        }
    }
}
