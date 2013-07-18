using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Screens;
using BombermanCommon.Resources;
using Bomberman.Content;
using Assets;
using BomberEngine.Core.Input;
using BomberEngine.Debugging;
using BomberEngine.Core.Visual;
using Bomberman.Game.Elements.Players.Input;
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

    public enum InputState
    {
        Available,
        Selected,
        Disabled
    }

    public class GameLobbyController : BombermanController
    {
        public enum ExitCode
        {
            StartGame,
            Cancel
        }

        private Scheme selectedScheme;

        private InputType[] inputTypes;
        private InputState[] inputStates;

        public GameLobbyController()
        {   
        }

        protected override void OnStart()
        {
            selectedScheme = Assets().GetScheme(A.maps_x);
            InitInputTypes(selectedScheme);

            StartScreen(new PlayersScreen(selectedScheme, inputTypes, InputTypeSelectDelegate, PlayersScreenButtonDelegate));
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Input types

        private void InitInputTypes(Scheme scheme)
        {
            int maxPlayers = scheme.GetMaxPlayersCount();

            inputTypes = new InputType[maxPlayers];
            for (int i = 0; i < inputTypes.Length; ++i)
            {
                inputTypes[i] = InputType.None;
            }

            inputStates = new InputState[(int)InputType.Count];

            SetInputState(InputType.None, InputState.Available);

            SetInputState(InputType.Keyboard1, InputState.Available);
            SetInputState(InputType.Keyboard2, InputState.Available);
            SetInputState(InputType.Keyboard3, InputState.Available);
            SetInputState(InputType.Keyboard4, InputState.Available);
            SetInputState(InputType.Keyboard5, InputState.Available);
            SetInputState(InputType.Keyboard6, InputState.Available);

            InputManager im = Input();
            SetInputState(InputType.GamePad1, im.IsGamePadConnected(0) ? InputState.Available : InputState.Disabled);
            SetInputState(InputType.GamePad2, im.IsGamePadConnected(1) ? InputState.Available : InputState.Disabled);
            SetInputState(InputType.GamePad3, im.IsGamePadConnected(2) ? InputState.Available : InputState.Disabled);
            SetInputState(InputType.GamePad4, im.IsGamePadConnected(3) ? InputState.Available : InputState.Disabled);

            SetInputState(InputType.Network, InputState.Disabled);
            SetInputState(InputType.Bot, InputState.Disabled);

            SetInputType(0, InputType.Keyboard1);
            SetInputType(1, InputType.Keyboard2);
        }

        private void InputTypeSelectDelegate(InputTypeView view, bool forward)
        {   
            InputType inputType = view.GetSelectedType();
            InputType newInputType = forward ? NextInputType(inputType) : PrevInputType(inputType);
            if (newInputType != inputType)
            {
                SetInputState(inputType, InputState.Available);
                SetInputType(view.index, newInputType);
                view.SetSelectedType(newInputType);
            }
        }

        private InputType NextInputType(InputType currentType)
        {
            int typesCount = (int)InputType.Count;
            int typeIndex = (int)currentType;
            for (int i = 1; i < typesCount; ++i)
            {
                int index = (typeIndex + i) % typesCount;
                InputType type = (InputType)index;

                switch (type)
                {
                    case InputType.None:
                    case InputType.Network:
                    case InputType.Bot:
                        break;

                    default:
                        {
                            if (inputStates[index] == InputState.Selected)
                            {
                                continue;
                            }
                            break;
                        }
                }

                if (inputStates[index] == InputState.Available)
                {
                    return (InputType)index;
                }
            }

            return currentType;
        }

        private InputType PrevInputType(InputType currentType)
        {
            int typesCount = (int)InputType.Count;
            int typeIndex = (int)currentType;
            for (int i = 1; i < typesCount; ++i)
            {
                int index = (typeIndex - i);
                if (index < 0) index = typesCount - i;

                InputType type = (InputType)index;

                switch (type)
                {
                    case InputType.None:
                    case InputType.Network:
                    case InputType.Bot:
                        break;

                    default:
                        {
                            if (inputStates[index] == InputState.Selected)
                            {
                                continue;
                            }
                            break;
                        }
                }

                if (inputStates[index] == InputState.Available)
                {
                    return (InputType)index;
                }
            }

            return currentType;
        }

        private void SetInputType(int slotIndex, InputType type)
        {
            Debug.Assert(inputStates[(int)type] == InputState.Available);
            inputTypes[slotIndex] = type;
            switch (type)
            {
                case InputType.None:
                case InputType.Network:
                case InputType.Bot:
                    break;
                default:
                    SetInputState(type, InputState.Selected);
                    break;
            }
        }

        private void SetInputState(InputType type, InputState state)
        {
            inputStates[(int)type] = state;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Button delegates

        private void PlayersScreenButtonDelegate(Button button)
        {
            PlayersScreen.ButtonId buttonId = (PlayersScreen.ButtonId)button.id;
            switch (buttonId)
            {
                case PlayersScreen.ButtonId.Start:
                    Stop(ExitCode.StartGame);
                    break;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public Scheme GetSelectedScheme()
        {
            return selectedScheme;
        }

        public GameSettings.InputEntry[] CreateInputEntries()
        {
            int count = 0;
            for (int i = 0; i < inputTypes.Length; ++i)
            {
                if (inputTypes[i] != InputType.None)
                {
                    ++count;
                }
            }

            GameSettings.InputEntry[] entries = new GameSettings.InputEntry[count];
            for (int i = 0, j = 0; i < inputTypes.Length && j < count; ++i)
            {
                if (inputTypes[i] != InputType.None)
                {
                    PlayerInput input = CreatePlayerInput(inputTypes[i]);
                    entries[i] = new GameSettings.InputEntry(i, input);
                }
            }

            return entries;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        private static PlayerKeyboardInput[] keyboardInputs;
        private static PlayerGamePadInput[] gamePadInputs;

        private PlayerInput CreatePlayerInput(InputType inputType)
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

        private PlayerKeyboardInput CreateKeyBoardInput(int index)
        {
            if (keyboardInputs == null)
            {
                keyboardInputs = new PlayerKeyboardInput[CVars.sy_maxKeyboards.intValue];
            }

            PlayerKeyboardInput input = keyboardInputs[index];
            if (input == null)
            {
                input = new PlayerKeyboardInput();
                keyboardInputs[index] = input;
            }

            return InitKeyboardInput(index, input);
        }

        private PlayerKeyboardInput InitKeyboardInput(int index, PlayerKeyboardInput input)
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

        private PlayerGamePadInput CreateGamePadInput(int index)
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

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        private void Stop(ExitCode code)
        {
            Stop((int)code);
        }

        #endregion
    }
}
