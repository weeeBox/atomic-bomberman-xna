using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Visual;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework.Input;

namespace BomberEngine.Debugging
{
    public class GameConsole : DrawableElement, KeyboardListener
    {
        private List<String> m_lines;
        private StringBuilder commandBuffer;

        private SpriteFont font;
        private float lineHeight;
        private float lineSpacing;

        private bool shiftPressed;

        public GameConsole(SpriteFont font)
            : base(640, 320)
        {
            this.font = font;

            m_lines = new List<String>();
            commandBuffer = new StringBuilder("> ");

            lineHeight = font.MeasureString("W").Y;
        }

        public override void Draw(Context context)
        {
            PreDraw(context);

            context.FillRect(0, 0, width, height, Color.Black);

            DrawLines(context);
            DrawPrompt(context);

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

        private void DrawPrompt(Context context)
        {
            float drawX = 10;
            float drawY = height - lineHeight - lineSpacing;

            context.DrawString(font, drawX, drawY, commandBuffer);
        }

        public void AddLine(String line)
        {
            m_lines.Add(line);
        }

        private void EnterChar(char chr)
        {
            commandBuffer.Append(chr);
        }

        public List<String> lines
        {
            get { return m_lines; }
        }

        public void KeyPressed(Keys key)
        {
            if (key >= Keys.A && key <= Keys.Z)
            {
                char chr = (char)key;
                if (shiftPressed)
                {
                    chr = char.ToUpper(chr);
                }

                EnterChar(chr);
            }
            else if (key == Keys.LeftShift || key == Keys.RightShift)
            {
                shiftPressed = true;
            }
        }

        public void KeyReleased(Keys key)
        {   
            if (key == Keys.LeftShift || key == Keys.RightShift)
            {
                shiftPressed = false;
            }
        }
    }
}
