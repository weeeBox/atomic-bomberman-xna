﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BomberEngine.Core.Visual.UI
{
    public class RectView : View
    {
        public Color borderColor;
        public Color fillColor;

        private RectView(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
        }

        public RectView(float x, float y, float width, float height, Color fillColor, Color borderColor)
            : base(x, y, width, height)
        {
            this.fillColor = fillColor;
            this.borderColor = borderColor;
        }

        public override void Draw(Context context)
        {
            PreDraw(context);
            context.FillRect(0, 0, width, height, fillColor);
            context.DrawRect(0, 0, width - 1, height - 1, borderColor);
            PostDraw(context);
        }
    }
}
