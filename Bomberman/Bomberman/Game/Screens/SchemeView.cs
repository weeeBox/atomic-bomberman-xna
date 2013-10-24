using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Visual;
using BombermanCommon.Resources.Scheme;
using Microsoft.Xna.Framework;
using BomberEngine.Core.Visual.UI;
using Bomberman.Content;
using BomberEngine.Core.Assets.Types;

namespace Bomberman.Game.Screens
{
    public class SchemeView : View
    {
        public enum Style
        {
            Small,
            Large
        }

        private static readonly Color COLOR_BACK = new Color(115, 89, 166);
        private const int FocusedBorder = 3;

        public SchemeView(Scheme scheme, Style style)
        {
            FieldData data = scheme.GetFieldData();
            Font nameFont;
            float dvy = 0.0f;
            FieldDataView.Style dataStyle;

            if (style == Style.Small)
            {
                SetSize(153, 143);
                AddView(new RectView(0, 0, width, height, COLOR_BACK, Color.Black));
                nameFont = Helper.fontSystem;
                dvy = 13.0f;
                dataStyle.width = 127;
                dataStyle.height = 86;
                dataStyle.iw = 119;
                dataStyle.ih = 79;
            }
            else if (style == Style.Large)
            {
                SetSize(215, 176);
                nameFont = Helper.fontSystem;
                dataStyle.width = 215;
                dataStyle.height = 145;
                dataStyle.iw = 201;
                dataStyle.ih = 132;
            }
            else
            {
                throw new ArgumentException("Unknown style: " + style);
            }

            FieldDataView dataView = new FieldDataView(data, dataStyle);
            dataView.x = 0.5f * width;
            dataView.y = dvy;
            dataView.alignX = View.ALIGN_CENTER;
            AddView(dataView);

            TextView nameView = new TextView(nameFont, scheme.GetName(), (int)width);
            nameView.x = 0.5f * width;
            nameView.y = 0.5f * (height + (dataView.y + dataView.height));
            nameView.alignX = nameView.alignY = View.ALIGN_CENTER;
            AddView(nameView);
        }

        public override void Draw(Context context)
        {
            PreDraw(context);
            if (focused)
            {
                context.FillRect(-FocusedBorder, -FocusedBorder, width + 2 * FocusedBorder, height + 2 * FocusedBorder, Color.White);
            }

            PostDraw(context);
        }
    }

    class FieldDataView : View
    {
        public struct Style
        {
            public float width;
            public float height;
            public float iw;
            public float ih;
        }

        private static readonly Color COLOR_SOLID = new Color(123, 123, 123);
        private static readonly Color COLOR_BRICK = new Color(255, 0, 255);

        public FieldDataView(FieldData data, Style style)
            : base(style.width, style.height)
        {
            AddView(new RectView(0, 0, width, height, Color.Gray, Color.Black));

            float iw = style.iw;
            float ih = style.ih;

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
