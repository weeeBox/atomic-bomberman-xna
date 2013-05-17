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
        public ButtonGroup(String[] titles, IButtonDelegate buttonDelegate)
            : this(titles, buttonDelegate, 0)
        {
        }

        public ButtonGroup(String[] titles, IButtonDelegate buttonDelegate, float width)
            : base(width, 0)
        {
            Font font = Helper.GetFont(A.fnt_button);

            foreach (String title in titles)
            {
                Button button = new Button(title, font, 0, 0, width, font.FontHeight());
                button.SetDelegate(buttonDelegate);
                AddView(button);
            }

            LayoutVer(10);
            ResizeToFitViewsVer();
        }
    }
}
