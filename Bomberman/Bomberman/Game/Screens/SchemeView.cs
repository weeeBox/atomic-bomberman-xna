using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Visual;
using BombermanCommon.Resources.Scheme;
using Microsoft.Xna.Framework;
using BomberEngine.Core.Visual.UI;
using Bomberman.Content;

namespace Bomberman.Game.Screens
{
    public class SchemeView : View
    {
        private static readonly Color COLOR_BACK = new Color(115, 89, 166);

        public SchemeView(Scheme scheme) :
            base(153, 143)
        {
            AddView(new RectView(0, 0, width, height, COLOR_BACK, Color.Black));

            FieldData data = scheme.GetFieldData();
            FieldDataView dataView = new FieldDataView(data);
            dataView.x = 0.5f * width;
            dataView.y = 13;
            dataView.alignX = View.ALIGN_CENTER;
            AddView(dataView);

            TextView nameView = new TextView(Helper.fontSystem, scheme.GetName(), (int)width);
            nameView.x = 0.5f * width;
            nameView.y = 0.5f * (height + (dataView.y + dataView.height));
            nameView.alignX = nameView.alignY = View.ALIGN_CENTER;
            AddView(nameView);
        }
    }

    class FieldDataView : View
    {
        private static readonly Color COLOR_SOLID = new Color(123, 123, 123);
        private static readonly Color COLOR_BRICK = new Color(255, 0, 255);

        public FieldDataView(FieldData data)
            : base(127, 86)
        {
            AddView(new RectView(0, 0, width, height, Color.Gray, Color.Black));

            float iw = 119;
            float ih = 79;

            RectView rect = new RectView(0.5f * (width - iw), 0.5f * (height - ih), iw, ih, Color.Green, Color.Green);
            AddView(rect);

            float cw = iw / data.GetWidth();
            float ch = ih / data.GetHeight();

            for (int y = 0; y < data.GetHeight(); ++y)
            {
                for (int x = 0; x < data.GetWidth(); ++x)
                {
                    FieldBlocks block = data.Get(x, y);
                    Color color;

                    switch (block)
                    {
                        case FieldBlocks.Brick:
                            color = COLOR_BRICK;
                            break;
                        case FieldBlocks.Solid:
                            color = COLOR_SOLID;
                            break;

                        default:
                            continue;
                    }

                    float cx = x * cw;
                    float cy = y * ch;

                    rect.AddView(new RectView(cx, cy, cw, ch, color, color));
                }
            }
        }
    }
}
