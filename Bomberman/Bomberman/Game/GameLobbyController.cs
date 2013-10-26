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
    public enum InputState
    {
        Available,
        Selected,
        Disabled
    }

    public class GameLobbyController : BmController
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
            StartScreen(new SchemePickScreen(MapScreenButtonDelegate));
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

        private void MapScreenButtonDelegate(Button button)
        {
            SchemePickScreen.ButtonId buttonId = (SchemePickScreen.ButtonId)button.id;
            switch (buttonId)
            {
                case SchemePickScreen.ButtonId.Scheme:

                    selectedScheme = (button as SchemeButton).scheme;
                    InitInputTypes(selectedScheme);

                    StartScreen(new PlayersScreen(selectedScheme, inputTypes, InputTypeSelectDelegate, PlayersScreenButtonDelegate));
                    break;
            }
        }

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
                    PlayerInput input = InputMapping.CreatePlayerInput(inputTypes[i]);
                    entries[i] = new GameSettings.InputEntry(i, input);
                }
            }

            return entries;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Helpers

        private void Stop(ExitCode code)
        {
            Stop((int)code);
        }

        #endregion
    }
}
