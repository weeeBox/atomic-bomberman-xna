using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BomberEngine.Core.Input
{
    public class InputManager : Updatable
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

        private const int MAX_GAMEPADS_COUNT = 4;

        private GamePadState[] currentGamepadStates;
        private KeyboardState currentKeyboardState;

        private KeyboardListener keyboardListener;
        private GamePadListener gamePadListener;
        private GamePadStateListener gamePadStateListener;
        private TouchListener touchListener;

        private GamePadDeadZone deadZone;

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
            if (gamePadStateListener != null)
            {
                if (IsControllerConnected(ref oldState, ref currentGamepadStates[gamePadIndex]))
                {
                    gamePadStateListener.GamePadConnected(gamePadIndex);
                }
                else if (IsControllerDisconnected(ref oldState, ref currentGamepadStates[gamePadIndex]))
                {
                    gamePadStateListener.GamePadConnected(gamePadIndex);
                }
            }

            if (gamePadListener != null)
            {
                for (int buttonIndex = 0; buttonIndex < CHECK_BUTTONS.Length; ++buttonIndex)
                {
                    Buttons button = CHECK_BUTTONS[buttonIndex];
                    if (IsButtonDown(button, ref oldState, ref currentGamepadStates[gamePadIndex]))
                    {
                        gamePadListener.ButtonPressed(new ButtonEvent(gamePadIndex, button));
                    }
                    else if (IsButtonUp(button, ref oldState, ref currentGamepadStates[gamePadIndex]))
                    {
                        gamePadListener.ButtonReleased(new ButtonEvent(gamePadIndex, button));
                    }
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
                        keyboardListener.KeyPressed(newKeys[i]);
                    }
                }
                for (int i = 0; i < oldKeys.Length; ++i)
                {
                    if (!newKeys.Contains(oldKeys[i]))
                    {
                        keyboardListener.KeyReleased(oldKeys[i]);
                    }
                }
            }
        }

        public bool IsKeyPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        #endregion
    }
}