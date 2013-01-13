using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Visual;
using Assets;
using Microsoft.Xna.Framework;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldDrawable : DrawableElement
    {
        private Field field;

        public FieldDrawable(Field field, int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            this.field = field;
        }

        public override void Draw(Context context)
        {
            PreDraw(context);

            DrawGrid(context);
            DrawBlocks(context);

            PostDraw(context);
        }

        private void DrawBlocks(Context context)
        {
            field.
        }

        private void DrawGrid(Context context)
        {
            int cellWidth = width / field.GetWidth();
            int cellHeight = height / field.GetHeight();

            for (int i = 0; i <= field.GetWidth(); ++i)
            {
                context.DrawLine(i * cellWidth, 0, i * cellWidth, height, Color.Gray);
            }

            for (int i = 0; i <= field.GetHeight(); ++i)
            {
                context.DrawLine(0, i * cellHeight, width, i * cellHeight, Color.Gray);
            }
        }
    }
}
