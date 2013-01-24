using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Visual;
using Microsoft.Xna.Framework.Graphics;

namespace BomberEngine.Debugging
{
    public class GameConsole : DrawableElement
    {   
        private List<String> m_lines;

        private SpriteFont font;
        private float lineHeight;
        private float lineSpacing;

        public GameConsole(SpriteFont font)
        {   
            this.font = font;

            m_lines = new List<String>();
            lineHeight = font.MeasureString("W").Y;
        }

        public override void Draw(Context context)
        {
            PreDraw(context);
            DrawLines(context);
            PostDraw(context);
        }

        private void DrawLines(Context context)
        {
            float drawX = 0;
            float drawY = 0;

            foreach (String line in m_lines)
            {
                context.DrawString(font, drawX, drawY, line);
                drawY += lineHeight + lineSpacing;
            }
        }

        public void AddLine(String line)
        {
            m_lines.Add(line);
        }

        public List<String> lines
        {
            get { return m_lines; }
        }
    }
}
