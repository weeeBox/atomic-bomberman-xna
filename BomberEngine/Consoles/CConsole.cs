using System;
using System.Collections.Generic;
using System.Text;
using BomberEngine.Consoles.Commands;
using BomberEngine.Core;
using BomberEngine.Core.Assets.Types;
using BomberEngine.Core.Events;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual;
using BomberEngine.Game;
using Microsoft.Xna.Framework;
using BomberEngine.Util;

namespace BomberEngine.Consoles
{
    public class CConsole : Screen
    {
        private List<String> m_lines;

        private const String PROMPT_STRING = "]";
        private StringBuilder commandBuffer;

        private int cursorPos;

        private Font font;
        private float charWidth;
        private float lineHeight;
        private float lineSpacing;

        private HashSet<KeyCode> additionalInputKeys;

        private CCommandRegister commands;
        private LinkedList<CCommand> suggestedCommands;

        private Color backColor;

        private bool carretVisible;

        public CConsole(Font font)
            : base(Application.GetWidth(), 0.5f * Application.GetHeight())
        {
            this.font = font;

            AllowsDrawPrevious = true;
            AllowsUpdatePrevious = true;

            m_lines = new List<String>();
            commands = new CCommandRegister();

            commandBuffer = new StringBuilder();
            suggestedCommands = new LinkedList<CCommand>();

            charWidth = font.StringWidth("W");
            lineHeight = font.FontHeight();
            InitAdditionalInputKeys();

            backColor = new Color(0.0f, 0.0f, 0.0f, 0.75f);
            carretVisible = true;

            ScheduleTimer(OnBlinkTimer, 0.25f, true);
        }

        public static CConsole Current()
        {
            return Application.RootController.console;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Drawable

        public override void Draw(Context context)
        {
            context.FillRect(0, 0, width, height, backColor);

            DrawLines(context);
            DrawPrompt(context);
        }

        private void DrawLines(Context context)
        {
            float drawX = 10;
            float drawY = height - 2 * (lineHeight + lineSpacing) - 10;

            for (int i = m_lines.Count - 1; i >= 0; --i)
            {
                String line = m_lines[i];

                font.DrawString(context, line, drawX, drawY);
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

            font.DrawString(context, PROMPT_STRING, drawX, drawY);
            drawX += PROMPT_STRING.Length * charWidth;

            if (carretVisible)
            {
                context.FillRect(drawX + cursorPos * charWidth, drawY + lineHeight, charWidth, 3, Color.White);
            }
            
            font.DrawString(context, commandBuffer.ToString(), drawX, drawY);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Command buffer

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

        private void DeletePrevChar()
        {
            if (cursorPos > 0)
            {
                commandBuffer.Remove(--cursorPos, 1);
            }
        }

        private void DeleteNextChar()
        {
            if (cursorPos < commandBuffer.Length)
            {
                commandBuffer.Remove(cursorPos, 1);
            }
        }

        private void TryExecuteCommand()
        {
            String commandString = commandBuffer.ToString();
            String[] args = commandString.Split(' ');

            if (args.Length > 0)
            {
                String name = args[0];
                CCommand command = commands.FindCommand(name);
                if (command != null)
                {
                    command.console = this;

                    if (args.Length > 1)
                    {
                        command.args = args;
                        command.Execute();
                    }
                    else
                    {
                        command.Execute();
                    }
                }
                else
                {
                    Append("Unknown command: '" + commandString + "'");
                }
            }

            SetCommandText("");
        }

        private void DoAutoComplete()
        {
            String token = commandBuffer.ToString();
            if (token.Length > 0)
            {
                suggestedCommands.Clear();
                commands.GetSuggested(token, suggestedCommands);

                if (suggestedCommands.Count == 1)
                {
                    CCommand command = suggestedCommands.First.Value;
                    SetCommandText(command.name, true);
                }
                else if (suggestedCommands.Count > 1)
                {
                    String suggestedText = GetSuggestedText(token, suggestedCommands);
                    SetCommandText(suggestedText, false);
                }
            }
        }

        private String GetSuggestedText(String token, LinkedList<CCommand> commandList)
        {
            LinkedListNode<CCommand> firstNode = commandList.First;
            String firstCommandName = firstNode.Value.name;

            if (firstCommandName.Length > token.Length)
            {
                StringBuilder suggestedToken = new StringBuilder(token);
                for (int i = token.Length; i < firstCommandName.Length; ++i)
                {
                    char chr = firstCommandName[i];
                    for (LinkedListNode<CCommand> nextNode = firstNode.Next; nextNode != null; nextNode = nextNode.Next)
                    {
                        String otherCommandName = nextNode.Value.name;
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

        private void Append(String line)
        {
            m_lines.Add(line);
        }

        private void Clear()
        {
            commandBuffer.Clear();
            cursorPos = 0;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Command register

        public bool RegisterCommand(CCommand command)
        {
            return commands.RegisterCommand(command);
        }

        public bool UnregisterCommand(CCommand command)
        {
            return commands.UnregisterCommand(command);
        }

        public List<CCommand> ListCommands()
        {
            return commands.ListCommands();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Printing

        public void Print(String message)
        {
            Append(message);
        }

        public void Print(String format, params Object[] args)
        {
            String message = StringUtils.TryFormat(format, args);
            Append(message);
        }

        public void PrintIndent(String message)
        {
            Print("  " + message);
        }

        public void PrintIndent(String format, params Object[] args)
        {
            Print("  " + format, args);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Cursor navigation 

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

        private void MoveCursorWordLeft()
        {
            while (cursorPos > 0 && commandBuffer[cursorPos - 1] == ' ')
            {
                --cursorPos;
            }

            while (cursorPos > 0 && commandBuffer[cursorPos-1] != ' ')
            {
                --cursorPos;
            }
        }

        private void MoveCursorWordRight()
        {
            while (cursorPos < commandBuffer.Length - 1 && commandBuffer[cursorPos + 1] == ' ' || cursorPos == commandBuffer.Length - 1)
            {
                ++cursorPos;
            }

            while (cursorPos < commandBuffer.Length - 1 && commandBuffer[cursorPos + 1] != ' ' || cursorPos == commandBuffer.Length - 1)
            {
                ++cursorPos;
            }

            while (cursorPos < commandBuffer.Length - 1 && commandBuffer[cursorPos + 1] == ' ' || cursorPos == commandBuffer.Length - 1)
            {
                ++cursorPos;
            }

            MoveCursorRight();
        }

        private void MoveCursorHome()
        {
            cursorPos = 0;
        }

        private void MoveCursorEnd()
        {
            cursorPos = commandBuffer.Length;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Timers

        private void OnBlinkTimer(Timer timer)
        {
            carretVisible = !carretVisible;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Handle events

        public override bool HandleEvent(Event evt)
        {
            if (evt.code == Event.KEY)
            {
                KeyEvent keyEvent = evt as KeyEvent;

                switch (keyEvent.state)
                {
                    case KeyEvent.PRESSED:
                    {
                        if (OnKeyPressed(ref keyEvent.arg)) return true;
                        break;
                    }

                    case KeyEvent.REPEATED:
                    {
                        if (OnKeyRepeat(ref keyEvent.arg)) return true;
                        break;
                    }

                    case KeyEvent.RELEASED:
                    {
                        if (OnKeyReleased(ref keyEvent.arg)) return true;
                        break;
                    }
                }
            }

            return base.HandleEvent(evt);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Input

        private bool OnKeyPressed(ref KeyEventArg e)
        {
            KeyCode key = e.key;

            if (e.IsCtrlPressed())
            {
                if (key == KeyCode.KB_Left)
                {
                    MoveCursorWordLeft();
                    return true;
                }

                if (key == KeyCode.KB_Right)
                {
                    MoveCursorWordRight();
                    return true;
                }

                if (key == KeyCode.KB_A)
                {
                    MoveCursorHome();
                    return true;
                }

                if (key == KeyCode.KB_E)
                {
                    MoveCursorEnd();
                    return true;
                }
            }

            if (key >= KeyCode.KB_A && key <= KeyCode.KB_Z)
            {
                char chr = (char)key;
                if (!e.IsShiftPressed())
                {
                    chr = char.ToLower(chr);
                }

                EnterChar(chr);
                return true;
            }

            if (key >= KeyCode.KB_D0 && key <= KeyCode.KB_D9 || key >= KeyCode.KB_NumPad0 && key <= KeyCode.KB_NumPad9 || key == KeyCode.KB_Space)
            {
                EnterChar((char)key);
                return true;
            }

            if (key == KeyCode.KB_Left)
            {
                MoveCursorLeft();
                return true;
            }

            if (key == KeyCode.KB_Right)
            {
                MoveCursorRight();
                return true;
            }

            if (key == KeyCode.KB_Back)
            {
                DeletePrevChar();
                return true;
            }

            if (key == KeyCode.KB_Delete)
            {
                DeleteNextChar();
                return true;
            }

            if (key == KeyCode.KB_Enter)
            {
                TryExecuteCommand();
                return true;
            }

            if (key == KeyCode.KB_Tab)
            {
                DoAutoComplete();
                return true;
            }

            if (key == KeyCode.KB_Home)
            {
                MoveCursorHome();
                return true;
            }

            if (key == KeyCode.KB_End)
            {
                MoveCursorEnd();
                return true;
            }

            if (key == KeyCode.KB_Escape)
            {
                Clear();
                return true;
            }

            return false;
        }

        private bool OnKeyRepeat(ref KeyEventArg e)
        {
            KeyCode key = e.key;

            if (key >= KeyCode.KB_A && key <= KeyCode.KB_Z || 
                key >= KeyCode.KB_D0 && key <= KeyCode.KB_D9 || 
                key >= KeyCode.KB_NumPad0 && key <= KeyCode.KB_NumPad9)
            {
                return OnKeyPressed(ref e);
            }

            if (key == KeyCode.KB_Left ||
                key == KeyCode.KB_Right ||
                key == KeyCode.KB_Back ||
                key == KeyCode.KB_Delete)
            {
                return OnKeyPressed(ref e);
            }

            return false;
        }

        private bool OnKeyReleased(ref KeyEventArg e)
        {
            return false;
        }

        private void InitAdditionalInputKeys()
        {
            additionalInputKeys = new HashSet<KeyCode>();
            additionalInputKeys.Add(KeyCode.KB_Space);
            additionalInputKeys.Add(KeyCode.KB_Multiply);
            additionalInputKeys.Add(KeyCode.KB_Add);
            additionalInputKeys.Add(KeyCode.KB_Separator);
            additionalInputKeys.Add(KeyCode.KB_Subtract);
            additionalInputKeys.Add(KeyCode.KB_Decimal);
            additionalInputKeys.Add(KeyCode.KB_Divide);
            additionalInputKeys.Add(KeyCode.KB_OemSemicolon);
            additionalInputKeys.Add(KeyCode.KB_OemPlus);
            additionalInputKeys.Add(KeyCode.KB_OemComma);
            additionalInputKeys.Add(KeyCode.KB_OemMinus);
            additionalInputKeys.Add(KeyCode.KB_OemPeriod);
            additionalInputKeys.Add(KeyCode.KB_OemQuestion);
            additionalInputKeys.Add(KeyCode.KB_OemOpenBrackets);
            additionalInputKeys.Add(KeyCode.KB_OemPipe);
            additionalInputKeys.Add(KeyCode.KB_OemCloseBrackets);
            additionalInputKeys.Add(KeyCode.KB_OemQuotes);
            additionalInputKeys.Add(KeyCode.KB_OemBackslash);
        }

        #endregion
    }
}
