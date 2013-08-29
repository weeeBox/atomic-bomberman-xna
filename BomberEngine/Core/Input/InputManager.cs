using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Debugging;
using System;
using BomberEngine.Game;
using BomberEngine.Core.Events;

namespace BomberEngine.Core.Input
{
    public struct KeyEventArg
    {
        public int playerIndex;
        public KeyCode key;

        public KeyEventArg(KeyCode key)
            : this(key, -1)
        {
        }

        public KeyEventArg(KeyCode key, int playerIndex)
        {
            this.key = key;
            this.playerIndex = playerIndex;
        }

        public bool IsShiftPressed()
        {
            return GetInputManager().IsShiftPressed();
        }

        public bool IsAltPressed()
        {
            return GetInputManager().IsAltPressed();
        }

        public bool IsCtrlPressed()
        {
            return GetInputManager().IsControlPressed();
        }

        public InputManager GetInputManager()
        {
            return Application.Input();
        }

        public static bool Equals(ref KeyEventArg a, ref KeyEventArg b)
        {
            return a.key == b.key && a.playerIndex == b.playerIndex;
        }
    }

    public class InputManager : BaseObject, IUpdatable
    {
        private static PlayerIndex[] PLAYERS_INDICES = 
        { 
            PlayerIndex.One, 
            PlayerIndex.Two, 
            PlayerIndex.Three, 
            PlayerIndex.Four 
        };

        private static Buttons[] CHECK_BUTTONS = 
        {
            Buttons.DPadUp,
            Buttons.DPadDown,
            Buttons.DPadLeft,
            Buttons.DPadRight,
            Buttons.Start,
            Buttons.Back,
            Buttons.LeftStick,
            Buttons.RightStick,
            Buttons.LeftShoulder,
            Buttons.RightShoulder,
            Buttons.BigButton,
            Buttons.A,
            Buttons.B,
            Buttons.X,
            Buttons.Y,
            Buttons.LeftThumbstickLeft,
            Buttons.RightTrigger,
            Buttons.LeftTrigger,
            Buttons.RightThumbstickUp,
            Buttons.RightThumbstickDown,
            Buttons.RightThumbstickRight,
            Buttons.RightThumbstickLeft,
            Buttons.LeftThumbstickUp,
            Buttons.LeftThumbstickDown,
            Buttons.LeftThumbstickRight,
        };

        private struct KeyRepeatInfo
        {
            public KeyEventArg eventArg;
            public float timestamp;

            public static readonly KeyRepeatInfo None = new KeyRepeatInfo(new KeyEventArg(KeyCode.None), Single.MaxValue);

            public KeyRepeatInfo(KeyEventArg eventArg, float timestamp)
            {
                this.eventArg = eventArg;
                this.timestamp = timestamp;
            }

            public static bool Equals(ref KeyRepeatInfo a, ref KeyRepeatInfo b)
            {
                return KeyEventArg.Equals(ref a.eventArg, ref b.eventArg);
            }
        }

        private const int MAX_GAMEPADS_COUNT = 4;

#if WINDOWS
        private const int REPEAT_KEYBOARD_INDEX = MAX_GAMEPADS_COUNT;
        private const int REPEAT_KEYS_COUNT = REPEAT_KEYBOARD_INDEX + 1;
#else
        private const int REPEAT_KEYS_COUNT = MAX_GAMEPADS_COUNT;
#endif

        private GamePadState[] currentGamepadStates;
        private KeyboardState currentKeyboardState;

        private KeyRepeatInfo[] keyRepeats;

        private List<IKeyInputListener> keyListeners;
        private List<ITouchInputListener> touchListeners;

        private GamePadDeadZone deadZone;

        private bool shiftPressed;
        private bool ctrlPressed;
        private bool altPressed;

        public InputManager()
        {
            deadZone = GamePadDeadZone.Circular;
            keyListeners = new List<IKeyInputListener>();
            touchListeners = new List<ITouchInputListener>();

            currentGamepadStates = new GamePadState[MAX_GAMEPADS_COUNT];
            for (int i = 0; i < currentGamepadStates.Length; ++i)
            {
                currentGamepadStates[i] = GamePad.GetState(PLAYERS_INDICES[i], deadZone);
            }

            keyRepeats = new KeyRepeatInfo[REPEAT_KEYS_COUNT];
            for (int i = 0; i < keyRepeats.Length; ++i)
            {
                keyRepeats[i] = KeyRepeatInfo.None;
            }

            currentKeyboardState = Keyboard.GetState();
        }

        public void Update(float delta)
        {
            UpdateGamepads();

#if WINDOWS
            UpdateKeyboard();
#endif   
            UpdateRepeats(delta);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Key repeats

        private void UpdateRepeats(float delta)
        {
            float currentTime = Application.CurrentTime;
            for (int i = 0; i < keyRepeats.Length; ++i)
            {
                if (keyRepeats[i].timestamp < currentTime)
                {
                    for (int j = 0; j < keyListeners.Count; ++j)
                    {
                        keyListeners[j].OnKeyRepeated(keyRepeats[i].eventArg);
                    }
                    keyRepeats[i].timestamp = currentTime + 0.03f;
                }
            }
        }

        private void SetKeyRepeat(ref KeyEventArg eventArg, int index)
        {   
            keyRepeats[index] = new KeyRepeatInfo(eventArg, Application.CurrentTime + 0.5f);
        }

        private void ClearKeyRepeat(ref KeyEventArg eventArg, int index)
        {
            if (KeyEventArg.Equals(ref eventArg, ref keyRepeats[index].eventArg))
            {   
                keyRepeats[index] = KeyRepeatInfo.None;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Gamepad

        private void UpdateGamepads()
        {
            for (int i = 0; i < MAX_GAMEPADS_COUNT; ++i)
            {
                UpdateGamepad(i);
            }
        }

        private void UpdateGamepad(int gamePadIndex)
        {
            GamePadState oldState = currentGamepadStates[gamePadIndex];
            currentGamepadStates[gamePadIndex] = GamePad.GetState(PLAYERS_INDICES[gamePadIndex], deadZone);

            bool connected = currentGamepadStates[gamePadIndex].IsConnected;

            if (IsGamePadConnected(ref oldState, ref currentGamepadStates[gamePadIndex]))
            {
                NotifyGamePadConnected(gamePadIndex);
            }
            else if (IsGamePadDisconnected(ref oldState, ref currentGamepadStates[gamePadIndex]))
            {
                NotifyGamePadDisconnected(gamePadIndex);
            }

            for (int buttonIndex = 0; buttonIndex < CHECK_BUTTONS.Length; ++buttonIndex)
            {
                Buttons button = CHECK_BUTTONS[buttonIndex];
                if (IsButtonDown(button, ref oldState, ref currentGamepadStates[gamePadIndex]))
                {
                    FireKeyPressed(gamePadIndex, KeyCodeHelper.FromButton(button), gamePadIndex);
                }
                else if (IsButtonUp(button, ref oldState, ref currentGamepadStates[gamePadIndex]))
                {
                    FireKeyReleased(gamePadIndex, KeyCodeHelper.FromButton(button), gamePadIndex);
                }
            }
        }

        private bool IsGamePadConnected(ref GamePadState oldState, ref GamePadState newState)
        {
            return newState.IsConnected && !oldState.IsConnected;
        }

        private bool IsGamePadDisconnected(ref GamePadState oldState, ref GamePadState newState)
        {
            return !newState.IsConnected && oldState.IsConnected;
        }

        private bool IsButtonDown(Buttons button, ref GamePadState oldState, ref GamePadState newState)
        {
            return newState.IsButtonDown(button) && oldState.IsButtonUp(button);
        }

        private bool IsButtonUp(Buttons button, ref GamePadState oldState, ref GamePadState newState)
        {
            return newState.IsButtonUp(button) && oldState.IsButtonDown(button);
        }

        public bool IsButtonPressed(int playerIndex, KeyCode code)
        {
            return currentGamepadStates[playerIndex].IsButtonDown(KeyCodeHelper.ToButton(code));
        }

        public bool IsGamePadConnected(int playerIndex)
        {
            return currentGamepadStates[playerIndex].IsConnected;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Gamepad analogs

        public GamePadThumbSticks ThumbSticks()
        {
            return ThumbSticks(0);
        }

        public GamePadThumbSticks ThumbSticks(int playerIndex)
        {
            return currentGamepadStates[playerIndex].ThumbSticks;
        }

        public GamePadTriggers Triggers()
        {
            return Triggers(0);
        }

        public GamePadTriggers Triggers(int playerIndex)
        {
            return currentGamepadStates[playerIndex].Triggers;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region GamePad state

        private void NotifyGamePadConnected(int playerIndex)
        {
            PostNotification(Notifications.GamePadConnected, playerIndex);
        }

        private void NotifyGamePadDisconnected(int playerIndex)
        {
            PostNotification(Notifications.GamePadDisconnected, playerIndex);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Keyboard

        private void UpdateKeyboard()
        {
            KeyboardState oldState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            Keys[] oldKeys = oldState.GetPressedKeys();
            Keys[] newKeys = currentKeyboardState.GetPressedKeys();

            for (int i = 0; i < newKeys.Length; ++i)
            {
                if (!oldKeys.Contains(newKeys[i]))
                {
                    UpdateModifierKeysPressed(ref newKeys[i]);
                    FireKeyPressed(-1, KeyCodeHelper.FromKey(newKeys[i]), REPEAT_KEYBOARD_INDEX);
                }
            }
            for (int i = 0; i < oldKeys.Length; ++i)
            {
                if (!newKeys.Contains(oldKeys[i]))
                {
                    UpdateModifierKeysReleased(ref oldKeys[i]);

                    FireKeyReleased(-1, KeyCodeHelper.FromKey(oldKeys[i]), REPEAT_KEYBOARD_INDEX);
                }
            }
        }

        public bool IsKeyPressed(KeyCode code)
        {
            return currentKeyboardState.IsKeyDown(KeyCodeHelper.ToKey(code));
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Key listener notifications

        private void FireKeyPressed(int playerIndex, KeyCode key, int index)
        {
            KeyEventArg eventArg = new KeyEventArg(key, playerIndex);
            for (int i = 0; i < keyListeners.Count; ++i)
            {
                keyListeners[i].OnKeyPressed(eventArg);
            }
            SetKeyRepeat(ref eventArg, index);
        }

        private void FireKeyReleased(int playerIndex, KeyCode key, int index)
        {
            KeyEventArg eventArg = new KeyEventArg(key, playerIndex);
            for (int i = 0; i < keyListeners.Count; ++i)
            {
                keyListeners[i].OnKeyReleased(eventArg);
            }
            ClearKeyRepeat(ref eventArg, index);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Modifier keys

        private void UpdateModifierKeysPressed(ref Keys key)
        {
            shiftPressed |= key == Keys.LeftShift || key == Keys.RightShift;
            altPressed |= key == Keys.LeftAlt || key == Keys.RightAlt;
            ctrlPressed |= key == Keys.LeftControl || key == Keys.RightControl;
        }

        private void UpdateModifierKeysReleased(ref Keys key)
        {
            shiftPressed &= key != Keys.LeftShift && key != Keys.RightShift;
            altPressed &= key != Keys.LeftAlt && key != Keys.RightAlt;
            ctrlPressed &= key != Keys.LeftControl && key != Keys.RightControl;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public void AddInputListener(IInputListener listener)
        {
            AddKeyboardListener(listener);
            AddTouchListener(listener);
        }

        public void AddKeyboardListener(IKeyInputListener listener)
        {
            Debug.Assert(!keyListeners.Contains(listener));
            keyListeners.Add(listener);
        }

        public void AddTouchListener(ITouchInputListener listener)
        {
            Debug.Assert(!touchListeners.Contains(listener));
            touchListeners.Add(listener);
        }

        public bool IsShiftPressed()
        {
            return shiftPressed;
        }

        public bool IsAltPressed()
        {
            return altPressed;
        }

        public bool IsControlPressed()
        {
            return ctrlPressed;
        }

        #endregion
    }
}