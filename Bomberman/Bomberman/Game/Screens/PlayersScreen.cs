using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using Assets;
using BomberEngine.Core.Assets.Types;
using Microsoft.Xna.Framework;
using BomberEngine.Core.Events;
using BomberEngine.Core.Input;
using Bomberman.Content;

namespace Bomberman.Game.Screens
{
    public class PlayersScreen : Screen
    {
        public PlayersScreen(Scheme selectedScheme)
        {
            View contentView = new View(64, 48, 521, 384);

            FooView fooView = new FooView(226, 48, 286, 363);
            
            contentView.AddView(fooView);

            AddView(contentView);
        }
    }

    enum InputType
    {
        Undefined,

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

    enum InputState
    {
        Available,
        Selected,
        Disabled
    }

    class FooView : View
    {   
        private InputState[] inputStates;
        
        public FooView(int x, int y, int w, int h)
            : base(x, y, w, h)
        {
            Font font = Helper.fontButton;

            inputStates = new InputState[(int)InputType.Count];

            SetInputState(InputType.Undefined, InputState.Available);

            SetInputState(InputType.Keyboard1, InputState.Available);
            SetInputState(InputType.Keyboard2, InputState.Available);
            SetInputState(InputType.Keyboard3, InputState.Available);
            SetInputState(InputType.Keyboard4, InputState.Available);
            SetInputState(InputType.Keyboard5, InputState.Available);
            SetInputState(InputType.Keyboard6, InputState.Available);

            InputManager im = Application.Input();
            SetInputState(InputType.GamePad1, im.IsGamePadConnected(0) ? InputState.Available : InputState.Disabled);
            SetInputState(InputType.GamePad2, im.IsGamePadConnected(1) ? InputState.Available : InputState.Disabled);
            SetInputState(InputType.GamePad3, im.IsGamePadConnected(2) ? InputState.Available : InputState.Disabled);
            SetInputState(InputType.GamePad4, im.IsGamePadConnected(3) ? InputState.Available : InputState.Disabled);

            SetInputState(InputType.Network, InputState.Disabled);
            SetInputState(InputType.Bot, InputState.Disabled);

            int maxPlayers = CVars.cg_maxPlayers.intValue;
            for (int i = 0; i < maxPlayers; ++i)
            {
                AddView(new FooRow(font, w, font.FontHeight(), inputStates));
            }

            LayoutVer(5);
            ResizeToFitViews();
        }

        private void SetInputState(InputType type, InputState state)
        {
            inputStates[(int)type] = state;
        }
    }

    class FooRow : View
    {
        private static IDictionary<InputType, String> inputTypeNameLookup;

        private int typeIndex;
        private InputState[] inputStates;
        private TextView typeView;

        public FooRow(Font font, int width, int height, InputState[] typeUsedFlag)
            : base(width, height)
        {
            this.inputStates = typeUsedFlag;

            focusable = true;

            View leftArrowView = new TextView(font, "<");
            AddView(leftArrowView);

            View rightArrowView = new TextView(font, ">");
            rightArrowView.x = width - rightArrowView.width;
            AddView(rightArrowView);

            typeIndex = (int)InputType.Undefined;
            typeView = new TextView(font, TypeIndexToString(typeIndex));
            typeView.alignX = View.ALIGN_CENTER;
            typeView.x = 0.5f * width;
            AddView(typeView);
        }

        public override bool HandleEvent(Event evt)
        {
            if (evt.code == Event.KEY)
            {
                KeyEvent keyEvent = evt as KeyEvent;
                if (keyEvent.arg.key == KeyCode.Left)
                {
                    if (keyEvent.state == KeyState.Pressed)
                        TrySwitchPrev();
                    return true;
                }

                if (keyEvent.arg.key == KeyCode.Right)
                {
                    if (keyEvent.state == KeyState.Pressed)
                        TrySwitchNext();
                    return true;
                }
            }

            return base.HandleEvent(evt);
        }

        private void TrySwitchNext()
        {
            int typesCount = (int)InputType.Count;
            for (int i = 1; i < typesCount; ++i)
            {
                int index = (typeIndex + i) % typesCount;
                InputType type = (InputType)index;

                switch (type)
                {
                    case InputType.Undefined:
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

                if (inputStates[index] == InputState.Disabled)
                {
                    continue;
                }

                inputStates[typeIndex] = InputState.Available;
                inputStates[index] = InputState.Selected;
                typeIndex = index;
                typeView.SetText(TypeIndexToString(typeIndex));
                break;
            }
        }

        private void TrySwitchPrev()
        {
            int typesCount = (int)InputType.Count;
            for (int i = 1; i < typesCount; ++i)
            {
                int index = (typeIndex - i);
                if (index < 0) index = typesCount - i;

                InputType type = (InputType)index;

                switch (type)
                {
                    case InputType.Undefined:
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

                if (inputStates[index] == InputState.Disabled)
                {
                    continue;
                }

                inputStates[typeIndex] = InputState.Available;
                inputStates[index] = InputState.Selected;
                typeIndex = index;
                typeView.SetText(TypeIndexToString(typeIndex));
                break;
            }
        }

        protected override void OnFocusChanged(bool focused)
        {
            this.color = focused ? Color.Yellow : Color.White;
        }

        private string TypeIndexToString(int index)
        {
            if (inputTypeNameLookup == null)
            {
                inputTypeNameLookup = new Dictionary<InputType, String>();
                inputTypeNameLookup[InputType.Undefined] = "..........";

                inputTypeNameLookup[InputType.Keyboard1] = "KEYBOARD 1";
                inputTypeNameLookup[InputType.Keyboard2] = "KEYBOARD 2";
                inputTypeNameLookup[InputType.Keyboard3] = "KEYBOARD 3";
                inputTypeNameLookup[InputType.Keyboard4] = "KEYBOARD 4";
                inputTypeNameLookup[InputType.Keyboard5] = "KEYBOARD 5";
                inputTypeNameLookup[InputType.Keyboard6] = "KEYBOARD 6";

                inputTypeNameLookup[InputType.GamePad1] = "GAMEPAD 1";
                inputTypeNameLookup[InputType.GamePad2] = "GAMEPAD 2";
                inputTypeNameLookup[InputType.GamePad3] = "GAMEPAD 3";
                inputTypeNameLookup[InputType.GamePad4] = "GAMEPAD 4";

                inputTypeNameLookup[InputType.Network] = "NETWORK";
                inputTypeNameLookup[InputType.Bot] = "BOT";
            }

            String name;
            if (inputTypeNameLookup.TryGetValue((InputType)index, out name))
            {
                return name;
            }

            return null;
        }
    }


}
