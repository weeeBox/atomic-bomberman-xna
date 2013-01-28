using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bomberman.Game.Scenes
{
    public class PauseScene : Scene
    {
        private Color backColor;

        public PauseScene()
        {
            backColor = new Color(0.0f, 0.0f, 0.0f, 0.25f);
            allowsDrawPrevious = true;
        }

        public override void Draw(Context context)
        {
            PreDraw(context);

            context.FillRect(0, 0, width, height, backColor);
            
            PostDraw(context);
        }

        public override void KeyPressed(Keys key)
        {
            if (key == Keys.Escape)
            {
                Finish();
            }
        }
    }
}
