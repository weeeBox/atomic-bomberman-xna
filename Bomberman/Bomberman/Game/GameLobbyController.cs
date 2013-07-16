using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Game.Screens;
using BombermanCommon.Resources;
using Bomberman.Content;
using Assets;
using BomberEngine.Core.Input;

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
            int maxPlayers = selectedScheme.GetMaxPlayersCount();

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

            StartScreen(new PlayersScreen(selectedScheme, InputTypeSelectDelegate));
        }

        private void InputTypeSelectDelegate(InputTypeView view, bool forward)
        {   
            InputType inputType = view.GetSelectedType();
            InputType newInputType = forward ? NextInputType(inputType) : PrevInputType(inputType);
            if (newInputType != inputType)
            {
                SetInputState(inputType, InputState.Available);
                SetInputState(newInputType, InputState.Selected);
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
            inputTypes[slotIndex] = type;
        }

        private void SetInputState(InputType type, InputState state)
        {
            inputStates[(int)type] = state;
        }

        private void Stop(ExitCode code)
        {
            Stop((int)code);
        }
    }
}
