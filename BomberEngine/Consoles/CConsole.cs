using System;
using System.Collections.Generic;
using System.Text;
using BomberEngine.Consoles;
using BomberEngine.Core;
using BomberEngine.Core.Assets.Types;
using BomberEngine.Core.Events;
using BomberEngine.Core.Input;
using BomberEngine.Core.Visual;
using BomberEngine.Game;
using Microsoft.Xna.Framework;
using BomberEngine.Util;
using BomberEngine.Debugging;

namespace BomberEngine.Consoles
{
    public class CConsole : Screen
    {
        private const String PROMPT_STRING = "]";
        private const String PROMPT_CMD_STRING = "]\\";

        private StringBuilder commandBuffer;
        private CConsoleHistory history;
        private CConsoleOutput output;

        private int cursorPos;

        private Font font;
        private float charWidth;
        private float lineHeight;
        private float lineSpacing;

        private CRegistery commands;
        private LinkedList<CCommand> suggestedCommands;

        private Color backColor;

        private bool carretVisible;

        private Dictionary<KeyCode, char> keyBindings;
        private Dictionary<KeyCode, char> keyShiftBindings;

        public CConsole(Font font)
            : base(Application.GetWidth(), 0.5f * Application.GetHeight())
        {
            this.font = font;

            AllowsDrawPrevious = true;
            AllowsUpdatePrevious = true;

            commands = new CRegistery();

            commandBuffer = new StringBuilder();

            history = new CConsoleHistory(128);
            output = new CConsoleOutput(512);

            suggestedCommands = new LinkedList<CCommand>();

            charWidth = font.StringWidth("W");
            lineHeight = font.FontHeight();
            lineSpacing = 0;

            backColor = new Color(0.0f, 0.0f, 0.0f, 0.75f);
            carretVisible = true;

            keyBindings = new Dictionary<KeyCode, char>();
            keyBindings[KeyCode.KB_OemMinus] = '-';
            keyBindings[KeyCode.KB_OemPlus] = '=';
            keyBindings[KeyCode.KB_OemComma] = ',';
            keyBindings[KeyCode.KB_OemPeriod] = '.';
            keyBindings[KeyCode.KB_OemQuestion] = '/';
            keyBindings[KeyCode.KB_OemOpenBrackets] = '[';
            keyBindings[KeyCode.KB_OemCloseBrackets] = ']';
            keyBindings[KeyCode.KB_OemQuotes] = '\\';
            keyBindings[KeyCode.KB_Divide] = '/';
            keyBindings[KeyCode.KB_Multiply] = '*';
            keyBindings[KeyCode.KB_Subtract] = '-';
            keyBindings[KeyCode.KB_Add] = '+';
            keyBindings[KeyCode.KB_OemSemicolon] = ';';
            keyBindings[KeyCode.KB_OemTilde] = '\'';
            keyBindings[KeyCode.KB_Decimal] = '.';
            keyBindings[KeyCode.KB_NumPad1] = '1';
            keyBindings[KeyCode.KB_NumPad2] = '2';
            keyBindings[KeyCode.KB_NumPad3] = '3';
            keyBindings[KeyCode.KB_NumPad4] = '4';
            keyBindings[KeyCode.KB_NumPad5] = '5';
            keyBindings[KeyCode.KB_NumPad6] = '6';
            keyBindings[KeyCode.KB_NumPad7] = '7';
            keyBindings[KeyCode.KB_NumPad8] = '8';
            keyBindings[KeyCode.KB_NumPad9] = '9';
            keyBindings[KeyCode.KB_NumPad0] = '0';


            keyShiftBindings = new Dictionary<KeyCode, char>();
            keyShiftBindings[KeyCode.KB_OemMinus] = '_';
            keyShiftBindings[KeyCode.KB_OemPlus] = '+';
            keyShiftBindings[KeyCode.KB_OemComma] = '<';
            keyShiftBindings[KeyCode.KB_OemPeriod] = '>';
            keyShiftBindings[KeyCode.KB_OemQuestion] = '?';
            keyShiftBindings[KeyCode.KB_D1] = '!';
            keyShiftBindings[KeyCode.KB_D2] = '@';
            keyShiftBindings[KeyCode.KB_D3] = '#';
            keyShiftBindings[KeyCode.KB_D4] = '$';
            keyShiftBindings[KeyCode.KB_D5] = '%';
            keyShiftBindings[KeyCode.KB_D6] = '^';
            keyShiftBindings[KeyCode.KB_D7] = '&';
            keyShiftBindings[KeyCode.KB_D8] = '*';
            keyShiftBindings[KeyCode.KB_D9] = '(';
            keyShiftBindings[KeyCode.KB_D0] = ')';
            keyShiftBindings[KeyCode.KB_OemOpenBrackets] = '{';
            keyShiftBindings[KeyCode.KB_OemCloseBrackets] = '}';
            keyShiftBindings[KeyCode.KB_OemQuotes] = '|';
            keyShiftBindings[KeyCode.KB_OemSemicolon] = ':';
            keyShiftBindings[KeyCode.KB_OemTilde] = '"';

            ScheduleTimer(OnBlinkTimer, 0.25f, true);
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

            for (LinkedListNode<String> node = output.lastNode; node != null; node = node.Previous)
            {
                String line = node.Value;

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
            String commandString = CommandBufferText().Trim();
            TryExecuteCommand(commandString, true);
            SetCommandText("");
        }

        public void TryExecuteCommand(String commandString)
        {
            TryExecuteCommand(commandString, false);
        }

        private void TryExecuteCommand(String commandString, bool verbose)
        {
            String[] args = commandString.Split(' ');

            if (args.Length > 0)
            {
                String name = args[0];

                CCommand command = commands.FindCommand(name);
                if (command != null)
                {
                    command.console = this;
                    command.args = args;

                    if (verbose)
                    {
                        Print(PROMPT_CMD_STRING + commandString);
                    }

                    if (args.Length > 1)
                    {
                        command.Execute();
                    }
                    else
                    {
                        command.Execute();
                    }
                }
                else
                {
                    Append("Unknown command: '" + name + "'");
                }

                if (verbose)
                {
                    PushHistory(commandString);
                }
            }
        }

        private void DoAutoComplete()
        {
            String token = CommandBufferText();
            if (token.Length > 0)
            {
                suggestedCommands.Clear();
                commands.GetSuggested(token, suggestedCommands);

                if (suggestedCommands.Count == 1)
                {
                    CCommand command = suggestedCommands.First.Value;
                    SetCommandText('\\' + command.name, true);
                }
                else if (suggestedCommands.Count > 1)
                {
                    String suggestedText = GetSuggestedText(token, suggestedCommands);
                    SetCommandText('\\' + suggestedText, false);

                    Print(PROMPT_CMD_STRING + suggestedText);
                    foreach (CCommand cmd in suggestedCommands)
                    {
                        PrintIndent(cmd.name);
                    }
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
                    char chrLower = char.ToLower(chr);
                    for (LinkedListNode<CCommand> nextNode = firstNode.Next; nextNode != null; nextNode = nextNode.Next)
                    {
                        String otherCommandName = nextNode.Value.name;
                        if (char.ToLower(otherCommandName[i]) != chrLower)
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

        private String CommandBufferText()
        {
            if (commandBuffer.Length > 1 && commandBuffer[0] == '\\')
            {
                return commandBuffer.ToString(1, commandBuffer.Length - 1);
            }

            return commandBuffer.Length == 0 ? "" : commandBuffer.ToString();
        }

        private void Append(String line)
        {
            output.Push(line);
        }

        private void Clear()
        {
            commandBuffer.Clear();
            cursorPos = 0;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Command register

        public bool RegisterCvar(CVar cvar)
        {
            return RegisterCommand(new CVarCommand(cvar));
        }

        public void RegisterCvars(CVar[] cvars)
        {
            foreach (CVar cvar in cvars)
            {
                RegisterCvar(cvar);
            }
        }

        public bool RegisterCommand(CCommand command)
        {
            return commands.RegisterCommand(command);
        }

        public bool RegisterCommands(CCommand[] commands)
        {
            for (int i = 0; i < commands.Length; ++i)
            {
                RegisterCommand(commands[i]);
            }

            return true;
        }

        public bool UnregisterCommand(CCommand command)
        {
            return commands.UnregisterCommand(command);
        }

        public bool UnregisterCommands(CCommand[] commands)
        {
            for (int i = 0; i < commands.Length; ++i)
            {
                UnregisterCommand(commands[i]);
            }
            return true;
        }

        public List<CCommand> ListCommands()
        {
            return commands.ListCommands();
        }

        public List<CCommand> ListCommands(String prefix)
        {
            return commands.ListCommands(prefix);
        }

        public List<CVar> ListVars()
        {
            return commands.ListVars();
        }

        public List<CVar> ListVars(String prefix)
        {
            return commands.ListVars(prefix);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region History

        private void PushHistory(String line)
        {
            history.Push(line);
        }

        private void PrevHistory()
        {
            String line = history.Prev();
            if (line != null)
            {
                SetCommandText(line);
            }
        }

        private void NextHistory()
        {
            String line = history.Next();
            if (line != null)
            {
                SetCommandText(line);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Output

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

        private void ScrollUp()
        {
            output.Up();
        }

        private void ScrollDown()
        {
            output.Down();
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

        private void OnBlinkTimer(DelayedCall timer)
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

                if (key == KeyCode.KB_Up)
                {
                    ScrollUp();
                    return true;
                }

                if (key == KeyCode.KB_Down)
                {
                    ScrollDown();
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

            if (e.IsShiftPressed())
            {
                char bindShiftChr;
                if (keyShiftBindings.TryGetValue(key, out bindShiftChr))
                {
                    EnterChar(bindShiftChr);
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

            if (key >= KeyCode.KB_D0 && key <= KeyCode.KB_D9 || key == KeyCode.KB_Space)
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

            if (key == KeyCode.KB_Up)
            {
                PrevHistory();
                return true;
            }

            if (key == KeyCode.KB_Down)
            {
                NextHistory();
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

            char bindChr;
            if (keyBindings.TryGetValue(key, out bindChr))
            {
                EnterChar(bindChr);
                return true;
            }

            return false;
        }

        private bool OnKeyRepeat(ref KeyEventArg e)
        {
            KeyCode key = e.key;

            if (key >= KeyCode.KB_A && key <= KeyCode.KB_Z || 
                key >= KeyCode.KB_D0 && key <= KeyCode.KB_D9)
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

            if (e.IsCtrlPressed())
            {
                if (key == KeyCode.KB_Up || key == KeyCode.KB_Down)
                {
                    return OnKeyPressed(ref e);
                }
            }

            if (keyBindings.ContainsKey(key))
            {
                return OnKeyPressed(ref e);
            }

            return false;
        }

        private bool OnKeyReleased(ref KeyEventArg e)
        {
            return false;
        }

        #endregion
    }
}
