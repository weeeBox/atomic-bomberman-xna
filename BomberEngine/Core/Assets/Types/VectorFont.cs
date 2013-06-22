using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BomberEngine.Core.Assets.Types
{   
    using BomberEngine.Core.Visual;
    using System.Collections.ObjectModel;

    public class VectorFont : Font
    {
        private SpriteFont fnt;
        private int fntHeight;
        private int charOffset;
        private char defaultChar;

        private IDictionary<char, int> charWidthLookup;

        public VectorFont(SpriteFont fnt)
        {
            this.fnt = fnt;
            fntHeight = (int)fnt.MeasureString("0").Y;
            charOffset = (int)fnt.Spacing;

            Nullable<char> defaultCharNullable = fnt.DefaultCharacter;
            defaultChar = defaultCharNullable.HasValue ? defaultCharNullable.Value : '?';
            charWidthLookup = CreateCharWidthLookup();
        }

        public override string[] WrapString(string text, int wrapWidth)
        {
            if (wrapWidth == Int32.MaxValue)
            {
                return new String[] { text };
            }
            return WrapString(text, wrapWidth, 200);
        }

        private IDictionary<char, int> CreateCharWidthLookup()
        {
            IDictionary<char, int> map = new Dictionary<char, int>();

            StringBuilder temp = new StringBuilder("0");
            ReadOnlyCollection<char> chars = fnt.Characters;
            foreach (char chr in chars)
            {
                temp[0] = chr;
                map[chr] = (int)fnt.MeasureString(temp).X;
            }

            return map;
        }

        private String[] WrapString(String text, int wrapWidth, int idxBufferSize)
        {
            int strLen = text.Length;
            int dataIndex = 0;
            int xc = 0;
            int wordWidth = 0;
            int strStartIndex = 0;
            int wordLastCharIndex = 0;
            int stringWidth = 0;
            int charIndex = 0;
            short[] strIdx = new short[idxBufferSize];
            while (charIndex < strLen)
            {
                int curCharIndex = charIndex;
                char curChar = text[curCharIndex];
                charIndex++;

                if (curChar == ' ' || curChar == '\n')
                {
                    wordLastCharIndex = curCharIndex;
                    if (stringWidth == 0 && wordWidth > 0)
                        wordWidth -= charOffset;

                    stringWidth += wordWidth;
                    wordWidth = 0;
                    xc = charIndex;

                    if (curChar == ' ')
                    {
                        xc--;
                        wordWidth = CharWidth(curChar) + charOffset;
                    }
                }
                else
                {
                    wordWidth += CharWidth(curChar) + charOffset;
                }

                if ((stringWidth + wordWidth) > wrapWidth && wordLastCharIndex != strStartIndex || curChar == '\n')
                {
                    strIdx[dataIndex++] = (short)strStartIndex;
                    strIdx[dataIndex++] = (short)wordLastCharIndex;

                    char tempChar;
                    while (xc < text.Length && (tempChar = text[xc]) == ' ')
                    {
                        wordWidth -= CharWidth(tempChar) + charOffset;
                        xc++;
                    }
                    wordWidth -= charOffset;

                    strStartIndex = xc;
                    wordLastCharIndex = strStartIndex;
                    stringWidth = 0;
                }
            }

            if (wordWidth != 0)
            {
                strIdx[dataIndex++] = (short)strStartIndex;
                strIdx[dataIndex++] = (short)strLen;
            }

            int strCount = dataIndex / 2;
            String[] strings = new String[strCount];
            for (int i = 0; i < strCount; i++)
            {
                int index = 2 * i;
                int start = strIdx[index];
                int end = strIdx[index + 1];

                strings[i] = text.Substring(start, end - start);
            }

            return strings;
        }

        public override int StringWidth(string str)
        {
            return (int)(fnt.MeasureString(str).X);
        }

        public override int CharWidth(char chr)
        {
            int width;
            if (charWidthLookup.TryGetValue(chr, out width))
            {
                return width;
            }

            return charWidthLookup[defaultChar];
        }

        public override int FontHeight()
        {
            return fntHeight;
        }

        public override int LineOffset()
        {
            return fnt.LineSpacing - fntHeight;
        }

        public override void DrawString(Context context, String text, float x, float y)
        {
            DrawString(context, text, x, y, TextAlign.LEFT | TextAlign.TOP);
        }

        public override void DrawString(Context context, String text, float x, float y, TextAlign textAlign)
        {
            float dx = x;
            float dy = y;

            if ((textAlign & TextAlign.RIGHT) != 0)
            {
                dx -= StringWidth(text);
            }
            else if ((textAlign & TextAlign.HCENTER) != 0)
            {
                dx -= 0.5f * StringWidth(text);
            }
            if ((textAlign & TextAlign.BOTTOM) != 0)
            {
                dy -= FontHeight();
            }
            else if ((textAlign & TextAlign.VCENTER) != 0)
            {
                dy -= 0.5f * FontHeight();
            }

            context.DrawString(fnt, dx, dy, text);
        }

        public SpriteFont Font
        {
            get { return fnt; }
        }
    }
}
