using System;
using BomberEngine;
using Bomberman.Content;
using Bomberman.Gameplay.Elements.Players;
using Bomberman.Gameplay.Screens;

namespace Bomberman.Gameplay
{
    public enum InputState
    {
        Available,
        Selected,
        Disabled
    }

    public class GameLobbyController : BmController
    {
        private static readonly String KeyLastPageIndex   = "LastPageIndex";
        private static readonly String KeyLastSchemeIndex = "LastMapIndex";

        public enum ExitCode
        {
            StartGame,
        }

        private Scheme selectedScheme;

        private InputType[] inputTypes;
        private InputState[] inputStates;

        public GameLobbyController()
        {   
        }

        protected override void OnStart()
        {
            int pageIndex = Application.Storage().GetInt(KeyLastPageIndex);
            int selectedIndex = Application.Storage().GetInt(KeyLastSchemeIndex);

            StartScreen(new SchemePickScreen(SchemePickButtonDelegate, pageIndex, selectedIndex));
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

            InputManager im = Input.Manager;
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
            Assert.IsTrue(inputStates[(int)type] == InputState.Available);
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

        private void SchemePickButtonDelegate(Button button)
        {
            SchemeButton schemeButton = button as SchemeButton;
            if (schemeButton != null)
            {
                SchemePickScreen screen = CurrentScreen() as SchemePickScreen;
                int pageIndex = screen.pageIndex;
                int selectedIndex = screen.selectedIndex;

                Application.Storage().Set(KeyLastPageIndex, pageIndex);
                Application.Storage().Set(KeyLastSchemeIndex, selectedIndex);

                selectedScheme = schemeButton.scheme;

                InitInputTypes(selectedScheme);

                StartNextScreen(new PlayersScreen(selectedScheme, inputTypes, InputTypeSelectDelegate, PlayersScreenButtonDelegate));
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

        public PlayerInput[] CreateInputEntries()
        {
            int count = 0;
            for (int i = 0; i < inputTypes.Length; ++i)
            {
                if (inputTypes[i] != InputType.None)
                {
                    ++count;
                }
            }

            PlayerInput[] entries = new PlayerInput[count];
            for (int i = 0, j = 0; i < inputTypes.Length && j < count; ++i)
            {
                if (inputTypes[i] != InputType.None)
                {
                    entries[i] = InputMapping.CreatePlayerInput(inputTypes[i]);
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
