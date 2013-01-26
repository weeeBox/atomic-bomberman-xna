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
using BomberEngine.Debugging.Commands;

namespace BomberEngine.Debugging
{
    public class GameConsole : DrawableElement, KeyboardListener
    {
        private List<String> m_lines;

        private const String PROMPT_STRING = "> ";
        private StringBuilder commandBuffer;

        private int cursorPos;

        private SpriteFont font;
        private float charWidth;
        private float lineHeight;
        private float lineSpacing;

        private bool shiftPressed;

        private HashSet<Keys> additionalInputKeys;

        private ConsoleCommandRegister commands;
        private LinkedList<ConsoleCommand> suggestedCommands;

        public GameConsole(SpriteFont font)
            : base(640, 320)
        {
            this.font = font;

            m_lines = new List<String>();
            commands = new ConsoleCommandRegister();

            commandBuffer = new StringBuilder();
            suggestedCommands = new LinkedList<ConsoleCommand>();

            Vector2 charSize = font.MeasureString("W");
            charWidth = charSize.X;
            lineHeight = charSize.Y;
            InitAdditionalInputKeys();
        }

        private void InitAdditionalInputKeys()
        {
            additionalInputKeys = new HashSet<Keys>();
            additionalInputKeys.Add(Keys.Space);
            additionalInputKeys.Add(Keys.Multiply);
            additionalInputKeys.Add(Keys.Add);
            additionalInputKeys.Add(Keys.Separator);
            additionalInputKeys.Add(Keys.Subtract);
            additionalInputKeys.Add(Keys.Decimal);
            additionalInputKeys.Add(Keys.Divide);
            additionalInputKeys.Add(Keys.OemSemicolon);
            additionalInputKeys.Add(Keys.OemPlus);
            additionalInputKeys.Add(Keys.OemComma);
            additionalInputKeys.Add(Keys.OemMinus);
            additionalInputKeys.Add(Keys.OemPeriod);
            additionalInputKeys.Add(Keys.OemQuestion);
            additionalInputKeys.Add(Keys.OemOpenBrackets);
            additionalInputKeys.Add(Keys.OemPipe);
            additionalInputKeys.Add(Keys.OemCloseBrackets);
            additionalInputKeys.Add(Keys.OemQuotes);
            additionalInputKeys.Add(Keys.OemBackslash);
        }

        public bool RegisterCommand(ConsoleCommand command)
        {
            return commands.RegisterCommand(command);
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
            float drawX = 10;
            float drawY = height - 2 * (lineHeight + lineSpacing) - 10;

            for (int i = m_lines.Count - 1; i >= 0; --i)
            {
                String line = m_lines[i];

                context.DrawString(font, drawX, drawY, line);
                drawY -= lineHeight + lineSpacing;

                if (drawY < 0)
                {
                    break;
                }
            }
        }

        private void DrawPrompt(Context context)
        {
            float drawX = 10;
            float drawY = height - lineHeight - lineSpacing - 10;

            context.DrawString(font, drawX, drawY, PROMPT_STRING);
            drawX += PROMPT_STRING.Length * charWidth;

            context.FillRect(drawX + cursorPos * charWidth, drawY + lineHeight, charWidth, 3, Color.White);
            context.DrawString(font, drawX, drawY, commandBuffer);
        }

        public void AddLine(String line)
        {
            m_lines.Add(line);
        }

        private void EnterChar(char chr)
        {
            commandBuffer.Insert(cursorPos++, chr);
        }

        private void SetCommandText(String text)
        {
            SetCommandText(text, false);
        }

        private void SetCommandText(String text, bool addSpace)
        {
            commandBuffer.Clear();
            commandBuffer.Append(text);
            if (addSpace)
            {
                commandBuffer.Append(" ");
            }

            cursorPos = commandBuffer.Length;
        }

        private void DeleteChar()
        {
            if (cursorPos > 0)
            {
                commandBuffer.Remove(--cursorPos, 1);
            }
        }

        private void TryExecuteCommand()
        {
            String commandString = commandBuffer.ToString();
            String[] tokens = commandString.Split(' ');

            if (tokens.Length > 0)
            {
                String name = tokens[0];
                ConsoleCommand command = commands.FindCommand(name);
                if (command != null)
                {
                    if (tokens.Length > 1)
                    {
                        String[] args = new String[tokens.Length - 1];
                        Array.Copy(tokens, 1, args, 0, args.Length);

                        command.Execute(args);
                    }
                    else
                    {
                        command.Execute();
                    }
                }
                else
                {
                    AddLine("Nicht verstehen: '" + commandString + "'");
                }
            }

            SetCommandText("");
        }

        private void DoAutoComplete()
        {
            String token = commandBuffer.ToString();

            suggestedCommands.Clear();
            commands.GetSuggested(token, suggestedCommands);

            if (suggestedCommands.Count == 1)
            {
                ConsoleCommand command = suggestedCommands.First.Value;
                SetCommandText(command.GetName(), true);
            }
            else if (suggestedCommands.Count > 1)
            {
                String suggestedText = GetSuggestedText(token, suggestedCommands);
                SetCommandText(suggestedText, false);
            }
        }

        private String GetSuggestedText(String token, LinkedList<ConsoleCommand> commandList)
        {
            LinkedListNode<ConsoleCommand> firstNode = commandList.First;
            String firstCommandName = firstNode.Value.GetName();

            if (firstCommandName.Length > token.Length)
            {
                StringBuilder suggestedToken = new StringBuilder(token);
                for (int i = token.Length; i < firstCommandName.Length; ++i)
                {
                    char chr = firstCommandName[i];
                    for (LinkedListNode<ConsoleCommand> nextNode = firstNode.Next; nextNode != null; nextNode = nextNode.Next)
                    {
                        String otherCommandName = nextNode.Value.GetName();
                        if (otherCommandName[i] != chr)
                        {
                            return suggestedToken.ToString();
                        }
                    }
                    suggestedToken.Append(chr);
                }

                return suggestedToken.ToString();
            }

            return token;
        }

        private void Clear()
        {
            commandBuffer.Clear();
            cursorPos = 0;
        }

        private void MoveCursorLeft()
        {
            if (cursorPos > 0)
            {
                --cursorPos;
            }
        }

        private void MoveCursorRight()
        {
            if (cursorPos < commandBuffer.Length)
            {
                ++cursorPos;
            }
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
                if (!shiftPressed)
                {
                    chr = char.ToLower(chr);
                }

                EnterChar(chr);
            }
            else if (key >= Keys.D0 && key <= Keys.D9 || key >= Keys.NumPad0 && key <= Keys.NumPad9)
            {
                EnterChar((char)key);
            }
            //else if (additionalInputKeys.Contains(key))
            //{
            //    EnterChar((char)key);
            //}
            else if (key == Keys.Left)
            {
                MoveCursorLeft();
            }
            else if (key == Keys.Right)
            {
                MoveCursorRight();
            }
            else if (key == Keys.Back)
            {
                DeleteChar();
            }
            else if (key == Keys.Enter)
            {
                TryExecuteCommand();
            }
            else if (key == Keys.Tab)
            {
                DoAutoComplete();
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
