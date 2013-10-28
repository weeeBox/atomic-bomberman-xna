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

    public class InputMapping
    {
        private static PlayerKeyInput[] keyboardInputs;
        private static PlayerGamePadInput[] gamePadInputs;

        public static PlayerInput CreatePlayerInput(InputType inputType)
        {
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
                        return CreateKeyBoardInput(index);
                    }

                case InputType.GamePad1:
                case InputType.GamePad2:
                case InputType.GamePad3:
                case InputType.GamePad4:
                    {
                        int index = inputType - InputType.GamePad1;
                        return CreateGamePadInput(index);
                    }

                case InputType.None:
                    {
                        throw new ArgumentException("Can't create input for 'none' type");
                    }
            }

            throw new NotImplementedException("Unsupported input type: " + inputType);
        }

        private static PlayerKeyInput CreateKeyBoardInput(int index)
        {
            if (keyboardInputs == null)
            {
                keyboardInputs = new PlayerKeyInput[CVars.sy_maxKeyboards.intValue];
            }

            PlayerKeyInput input = keyboardInputs[index];
            if (input == null)
            {
                input = new PlayerKeyInput();
                keyboardInputs[index] = InitKeyboardInput(index, input);
            }

            return input;
        }

        private static PlayerKeyInput InitKeyboardInput(int index, PlayerKeyInput input)
        {
            // TODO: don't hard code
            switch (index)
            {
                case 0:
                    {
                        input.Map(KeyCode.W, PlayerAction.Up);
                        input.Map(KeyCode.A, PlayerAction.Left);
                        input.Map(KeyCode.S, PlayerAction.Down);
                        input.Map(KeyCode.D, PlayerAction.Right);
                        input.Map(KeyCode.OemCloseBrackets, PlayerAction.Bomb);
                        input.Map(KeyCode.OemOpenBrackets, PlayerAction.Special);
                        break;
                    }
                case 1:
                    {
                        input.Map(KeyCode.Up, PlayerAction.Up);
                        input.Map(KeyCode.Left, PlayerAction.Left);
                        input.Map(KeyCode.Down, PlayerAction.Down);
                        input.Map(KeyCode.Right, PlayerAction.Right);
                        input.Map(KeyCode.M, PlayerAction.Bomb);
                        input.Map(KeyCode.N, PlayerAction.Special);
                        break;
                    }
            }

            return input;
        }

        private static PlayerGamePadInput CreateGamePadInput(int index)
        {
            if (gamePadInputs == null)
            {
                gamePadInputs = new PlayerGamePadInput[CVars.sy_maxControllers.intValue];
            }

            PlayerGamePadInput input = gamePadInputs[index];
            if (input == null)
            {
                input = new PlayerGamePadInput(index);
                gamePadInputs[index] = input;
            }

            return input;
        }
    }
}
