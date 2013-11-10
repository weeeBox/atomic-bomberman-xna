using System;
using BomberEngine;
using Bomberman.Game.Elements.Players;

namespace Bomberman.Game
{
    public enum InputType
    {
        None,

        Keyboard1,
        Keyboard2,
        Keyboard3,
        Keyboard4,
        Keyboard5,
        Keyboard6,

        GamePad1,
        GamePad2,
        GamePad3,
        GamePad4,

        Network,
        Bot,

        Count
    }

    public class InputMapping : IDisposable
    {
        private static InputMapping instance;

        private PlayerKeyInput[] keyboardInputs;
        private PlayerGamePadInput[] gamePadInputs;

        public InputMapping()
        {
            instance = this;

            keyboardInputs = new PlayerKeyInput[6];
            for (int i = 0; i < keyboardInputs.Length; ++i)
            {
                keyboardInputs[i] = new PlayerKeyInput();
            }

            gamePadInputs = new PlayerGamePadInput[4];
            for (int i = 0; i < gamePadInputs.Length; ++i)
            {
                gamePadInputs[i] = new PlayerGamePadInput(i);
            }
        }

        public void Dispose()
        {
            if (this == instance)
            {
                instance = null;
            }
        }

        private PlayerInput CreatePlayerInputHelper(InputType inputType)
        {
            PlayerInput input;

            switch (inputType)
            {
                case InputType.Keyboard1:
                case InputType.Keyboard2:
                case InputType.Keyboard3:
                case InputType.Keyboard4:
                case InputType.Keyboard5:
                case InputType.Keyboard6:
                {
                    int index = inputType - InputType.Keyboard1;
                    input = keyboardInputs[index];
                    break;
                }

                case InputType.GamePad1:
                case InputType.GamePad2:
                case InputType.GamePad3:
                case InputType.GamePad4:
                {
                    int index = inputType - InputType.GamePad1;
                    input = gamePadInputs[index];
                    break;
                }

                case InputType.None:
                {
                    throw new ArgumentException("Can't create input for 'none' type");
                }

                default:
                {
                    throw new NotImplementedException("Unsupported input type: " + inputType);
                }
            }

            input.Reset();
            return input;
        }

        private void SetKeyboardActionHelper(int index, PlayerAction action, bool flag)
        {
            Assert.IsIndex(index, keyboardInputs);
            keyboardInputs[index].actionsArray[(int)action] = flag;
        }

        public static PlayerInput CreatePlayerInput(InputType inputType)
        {
            return instance.CreatePlayerInputHelper(inputType);
        }

        public static void SetKeyboardAction(int index, PlayerAction action, bool flag)
        {
            instance.SetKeyboardActionHelper(index, action, flag);
        }
    }
}
