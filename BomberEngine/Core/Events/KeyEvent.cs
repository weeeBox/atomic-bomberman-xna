using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core.Input;
using BomberEngine.Game;

namespace BomberEngine.Core.Events
{
    public enum KeyState
    {
        Pressed,
        Repeated,
        Released
    }

    public class KeyEvent : Event
    {
        public KeyEventArg arg;
        public KeyState state;

        public KeyEvent()
            : base(Event.KEY)
        {
        }

        public KeyEvent Init(KeyEventArg arg, KeyState state)
        {
            this.arg = arg;
            this.state = state;

            return this;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        public bool IsKeyPressed(KeyCode key)
        {
            return IsKeyState(KeyState.Pressed, key);
        }

        public bool IsKeyRepeated(KeyCode key)
        {
            return IsKeyState(KeyState.Repeated, key);
        }

        public bool IsKeyReleased(KeyCode key)
        {
            return IsKeyState(KeyState.Released, key);
        }

        public bool IsOneOfKeysPressed(params KeyCode[] keys)
        {
            return IsOneOfKeysState(KeyState.Pressed, keys);
        }

        public bool IsOneOfKeysRepeated(params KeyCode[] keys)
        {
            return IsOneOfKeysState(KeyState.Repeated, keys);
        }

        public bool IsOneOfKeysReleased(params KeyCode[] keys)
        {
            return IsOneOfKeysState(KeyState.Released, keys);
        }

        public bool IsCtrlPressed()
        {
            return Input.IsKeyPressed(KeyCode.LeftControl) || Input.IsKeyPressed(KeyCode.RightControl);
        }

        public bool IsAltPressed()
        {
            return Input.IsKeyPressed(KeyCode.LeftAlt) || Input.IsKeyPressed(KeyCode.RightAlt);
        }

        public bool IsShiftPressed()
        {
            return Input.IsKeyPressed(KeyCode.LeftShift) || Input.IsKeyPressed(KeyCode.RightShift);
        }

        private bool IsKeyState(KeyState s, KeyCode key)
        {
            return state == s && arg.key == key;
        }

        private bool IsOneOfKeysState(KeyState s, KeyCode[] keys)
        {
            if (state == s)
            {
                for (int i = 0; i < keys.Length; ++i)
                {
                    if (keys[i] == arg.key)
                        return true;
                }
            }

            return false;
        }

        public InputManager Input
        {
            get { return Application.Input(); }
        }

        #endregion
    }
}
