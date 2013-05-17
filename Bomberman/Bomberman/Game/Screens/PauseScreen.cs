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
    public class PauseScreen : Screen, IButtonDelegate
    {
        public PauseScreen()
        {   
            allowsDrawPrevious = true;

            Color backColor = new Color(0.0f, 0.0f, 0.0f, 0.25f);
            AddView(new RectView(0, 0, width, height, backColor, Color.Black));

            ButtonGroup group = new ButtonGroup(new String[] { "Resume", "Exit" }, this);
            group.alignX = group.alignY = View.ALIGN_CENTER;
            group.x = 0.5f * width;
            group.y = 0.5f * height;
            AddView(group);
        }

        public override bool OnKeyPressed(Keys key)
        {
            if (key == Keys.Escape)
            {
                Finish();
                return true;
            }

            return base.OnKeyPressed(key);
        }

        public override bool OnButtonPressed(ButtonEvent e)
        {
            if (e.button == Buttons.Back)
            {
                Finish();
                return true;
            }

            return base.OnButtonPressed(e);
        }

        public void OnButtonPress(AbstractButton button)
        {
            
        }
    }
}
