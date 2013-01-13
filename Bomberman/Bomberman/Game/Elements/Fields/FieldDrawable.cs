using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Visual;
using Assets;
using Microsoft.Xna.Framework;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldDrawable : VisualElement
    {
        private Field field;

        private int cellWidth;
        private int cellHeight;

        public FieldDrawable(Field field, int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            this.field = field;

            cellWidth = width / field.GetWidth();
            cellHeight = height / field.GetHeight();
        }

        public override void Draw(Context context)
        {
            PreDraw(context);
            
            for (int i = 0; i <= field.GetWidth(); ++i)
            {
                context.DrawLine(i * cellWidth, 0, i * cellWidth, height, Color.Gray);
            }

            for (int i = 0; i <= field.GetHeight(); ++i)
            {
                context.DrawLine(0, i * cellHeight, width, i * cellHeight, Color.Gray);
            }

            PostDraw(context);
        }
    }
}
