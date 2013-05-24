using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Debugging;
using System;

namespace BomberEngine.Core.Input
{
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

        private struct ButtonRepeat
        {   
            public int playerIndex;
            public Buttons button;
            public double repeatTime;

            public ButtonRepeat(int playerIndex, Buttons button, double repeatTime)
            {   
                this.playerIndex = playerIndex;
                this.button = button;
                this.repeatTime = repeatTime;
            }
        }

        private struct KeyRepeat
        {
            public Keys key;
            public double repeatTime;

            public KeyRepeat(Keys key, double repeatTime)
            {
                this.key = key;
                this.repeatTime = repeatTime;
            }
        }

        private const int MAX_GAMEPADS_COUNT = 4;

        private GamePadState[] currentGamepadStates;
        private KeyboardState currentKeyboardState;

        private IKeyboardListener keyboardListener;
        private IGamePadListener gamePadListener;
        private IGamePadStateListener gamePadStateListener;
        private ITouchListener touchListener;

        private GamePadDeadZone deadZone;

        private const float REPEAT_TIMEOUT = 0.5f;
        private double currentTime;

        private List<ButtonRepeat> buttonRepeats = new List<ButtonRepeat>();
        private List<KeyRepeat> keyRepeats = new List<KeyRepeat>();

        public InputManager()
        {
            deadZone = GamePadDeadZone.Circular;
            currentGamepadStates = new GamePadState[MAX_GAMEPADS_COUNT];

            for (int i = 0; i < currentGamepadStates.Length; ++i)
            {
                currentGamepadStates[i] = GamePad.GetState(PLAYERS_INDICES[i], deadZone);
            }

            currentKeyboardState = Keyboard.GetState();
        }

        public void Update(float delta)
        {
            UpdateGamepads();

#if WINDOWS
            UpdateKeyboard();
#endif

            UpdateRepeat(delta);
        }

        private void UpdateRepeat(float delta)
        {
            currentTime += delta;

            UpdateGamepadsRepeat(delta);

#if WINDOWS
            UpdateKeyboardRepeat(delta);
#endif
        }

        private void UpdateGamepadsRepeat(float delta)
        {
            for (int i = buttonRepeats.Count - 1; i >= 0; --i)
            {
                if (buttonRepeats[i].repeatTime <= currentTime)
                {
                    buttonRepeats.RemoveAt(i);
                    throw new NotImplementedException();
                }
            }
        }

        private void UpdateKeyboardRepeat(float delta)
        {
            for (int i = keyRepeats.Count - 1; i >= 0; --i)
            {
                if (keyRepeats[i].repeatTime <= currentTime)
                {
                    keyboardListener.OnKeyReleased(keyRepeats[i].key);
                    keyRepeats.RemoveAt(i);
                }
            }
        }

        private void AddKeyRepeat(Keys key)
        {
            Debug.Assert(Debug.flag && FindKeyRepeat(key) == -1);
            keyRepeats.Add(new KeyRepeat(key, currentTime + REPEAT_TIMEOUT));
        }

        private void RemoveKeyRepeat(Keys key)
        {
            int index = FindKeyRepeat(key);
            Debug.Assert(index != -1);

            if (index != -1)
            {
                keyRepeats.RemoveAt(index);
            }
        }

        private void AddButtonRepeat(int playerIndex, Buttons button)
        {
            Debug.Assert(Debug.flag && FindButtonRepeat(playerIndex, button) == -1);
            buttonRepeats.Add(new ButtonRepeat(playerIndex, button, currentTime + REPEAT_TIMEOUT));
        }

        private void RemoveButtonRepeat(int playerIndex, Buttons button)
        {
            int index = FindButtonRepeat(playerIndex, button);
            Debug.Assert(index != -1);

            if (index != -1)
            {
                buttonRepeats.RemoveAt(index);
            }
        }

        private int FindKeyRepeat(Keys key)
        {
            for (int i = 0; i < keyRepeats.Count; ++i)
            {
                if (keyRepeats[i].key == key)
                {
                    return i;
                }
            }
            return -1;
        }

        private int FindButtonRepeat(int playerIndex, Buttons button)
        {
            for (int i = 0; i < buttonRepeats.Count; ++i)
            {
                if (buttonRepeats[i].playerIndex == playerIndex && buttonRepeats[i].button == button)
                {
                    return i;
                }
            }
            return -1;
        }

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
                    gamePadListener.OnButtonPressed(new ButtonEventArg(gamePadIndex, button));
                    AddButtonRepeat(gamePadIndex, button);
                }
                else if (IsButtonUp(button, ref oldState, ref currentGamepadStates[gamePadIndex]))
                {
                    gamePadListener.OnButtonReleased(new ButtonEventArg(gamePadIndex, button));
                    RemoveButtonRepeat(gamePadIndex, button);
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

            if (keyboardListener != null)
            {
                Keys[] oldKeys = oldState.GetPressedKeys();
                Keys[] newKeys = currentKeyboardState.GetPressedKeys();

                for (int i = 0; i < newKeys.Length; ++i)
                {
                    if (!oldKeys.Contains(newKeys[i]))
                    {
                        keyboardListener.OnKeyPressed(newKeys[i]);
                        AddKeyRepeat(newKeys[i]);
                    }
                }
                for (int i = 0; i < oldKeys.Length; ++i)
                {
                    if (!newKeys.Contains(oldKeys[i]))
                    {
                        keyboardListener.OnKeyReleased(oldKeys[i]);
                        RemoveKeyRepeat(oldKeys[i]);
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

        #region Properties

        public void SetInputListener(IInputListener listener)
        {
            SetKeyboardListener(listener);
            SetGamePadListener(listener);
            SetGamePadStateListener(listener);
            SetTouchListener(listener);
        }

        public IKeyboardListener GetKeyboardListener()
        {
            return keyboardListener;
        }

        public void SetKeyboardListener(IKeyboardListener listener)
        {
            keyboardListener = listener;
        }

        public IGamePadListener GamePadListener()
        {
            return gamePadListener;
        }

        public void SetGamePadListener(IGamePadListener listener)
        {
            gamePadListener = listener;
        }

        public IGamePadStateListener GetGamePadStateListener()
        {
            return gamePadStateListener;
        }

        private void SetGamePadStateListener(IGamePadStateListener listener)
        {
            this.gamePadStateListener = listener;
        }

        public ITouchListener GetTouchListener()
        {
            return touchListener;
        }

        public void SetTouchListener(ITouchListener listener)
        {
            this.touchListener = listener;
        }

        #endregion
    }
}