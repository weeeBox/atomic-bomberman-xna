using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Assets.Types;
using Bomberman.Game;
using Assets;
using Bomberman.UI;

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

        public Button AddButton(String title, int id, ButtonDelegate buttonDelegate)
        {
            Button button = new TempButton(title);
            button.id = id;
            button.buttonDelegate = buttonDelegate;
            AddView(button);

            LayoutVer(10);
            ResizeToFitViewsVer();

            return button;
        }
    }
}
