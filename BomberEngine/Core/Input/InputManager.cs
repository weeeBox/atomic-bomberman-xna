using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Debugging;
using System;
using BomberEngine.Game;

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

        public static bool Equals(ref KeyEventArg a, ref KeyEventArg b)
        {
            return a.key == b.key && a.playerIndex == b.playerIndex;
        }
    }

    public class InputManager : IUpdatable
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
            public double timestamp;

            public static readonly KeyRepeatInfo None = new KeyRepeatInfo(new KeyEventArg(KeyCode.KB_None), Double.MaxValue);

            public KeyRepeatInfo(KeyEventArg eventArg, double timestamp)
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

        private IKeyInputListener keyListener;
        private ITouchInputListener touchListener;
        private IGamePadStateListener gamePadStateListener;

        private GamePadDeadZone deadZone;

        public InputManager()
        {
            deadZone = GamePadDeadZone.Circular;

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
            double currentTime = Application.CurrentTime();
            for (int i = 0; i < keyRepeats.Length; ++i)
            {
                if (keyRepeats[i].timestamp < currentTime)
                {
                    keyListener.OnKeyRepeated(keyRepeats[i].eventArg);
                    keyRepeats[i].timestamp = currentTime + 0.03;
                }
            }
        }

        private void SetKeyRepeat(ref KeyEventArg eventArg, int index)
        {   
            keyRepeats[index] = new KeyRepeatInfo(eventArg, Application.CurrentTime() + 0.5);
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

            if (IsControllerConnected(ref oldState, ref currentGamepadStates[gamePadIndex]))
            {
                gamePadStateListener.OnGamePadConnected(gamePadIndex);
            }
            else if (IsControllerDisconnected(ref oldState, ref currentGamepadStates[gamePadIndex]))
            {
                gamePadStateListener.OnGamePadConnected(gamePadIndex);
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

        private bool IsControllerConnected(ref GamePadState oldState, ref GamePadState newState)
        {
            return newState.IsConnected && !oldState.IsConnected;
        }

        private bool IsControllerDisconnected(ref GamePadState oldState, ref GamePadState newState)
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

        public bool IsControllerConnected(int playerIndex)
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

        #region Keyboard

        private void UpdateKeyboard()
        {
            KeyboardState oldState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            if (keyListener != null)
            {
                Keys[] oldKeys = oldState.GetPressedKeys();
                Keys[] newKeys = currentKeyboardState.GetPressedKeys();

                for (int i = 0; i < newKeys.Length; ++i)
                {
                    if (!oldKeys.Contains(newKeys[i]))
                    {   
                        FireKeyPressed(-1, KeyCodeHelper.FromKey(newKeys[i]), REPEAT_KEYBOARD_INDEX);
                    }
                }
                for (int i = 0; i < oldKeys.Length; ++i)
                {
                    if (!newKeys.Contains(oldKeys[i]))
                    {
                        FireKeyReleased(-1, KeyCodeHelper.FromKey(oldKeys[i]), REPEAT_KEYBOARD_INDEX);
                    }
                }
            }
        }

        public bool IsKeyPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Key listener notifications

        private void FireKeyPressed(int playerIndex, KeyCode key, int index)
        {
            KeyEventArg eventArg = new KeyEventArg(key, playerIndex);
            keyListener.OnKeyPressed(eventArg);
            SetKeyRepeat(ref eventArg, index);
        }

        private void FireKeyReleased(int playerIndex, KeyCode key, int index)
        {
            KeyEventArg eventArg = new KeyEventArg(key, playerIndex);
            keyListener.OnKeyReleased(eventArg);
            ClearKeyRepeat(ref eventArg, index);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public void SetInputListener(IInputListener listener)
        {
            SetKeyboardListener(listener);
            SetGamePadStateListener(listener);
            SetTouchListener(listener);
        }

        public IKeyInputListener GetKeyboardListener()
        {
            return keyListener;
        }

        public void SetKeyboardListener(IKeyInputListener listener)
        {
            keyListener = listener;
        }

        public IGamePadStateListener GetGamePadStateListener()
        {
            return gamePadStateListener;
        }

        private void SetGamePadStateListener(IGamePadStateListener listener)
        {
            this.gamePadStateListener = listener;
        }

        public ITouchInputListener GetTouchListener()
        {
            return touchListener;
        }

        public void SetTouchListener(ITouchInputListener listener)
        {
            this.touchListener = listener;
        }

        #endregion
    }
}