using System;
using System.Collections.Generic;
using BomberEngine;
using Bomberman.Content;
using Bomberman.UI;
using Microsoft.Xna.Framework;

namespace Bomberman.Game.Screens
{
    public class PlayersScreen : Screen
    {
        public enum ButtonId
        {
            Start,
            Back
        }

        public PlayersScreen(Scheme scheme, InputType[] inputTypes, InputTypeSelectDelegate selectDelegate, ButtonDelegate buttonDelegate)
        {
            View contentView = new View(64, 48, 521, 363);

            // scheme view
            contentView.AddView(new SchemeView(scheme, SchemeView.Style.Large));

            // input type selector
            int maxPlayers = inputTypes.Length;
            View inputTypeContainer = new View(226, 0, 286, 0);
            Font font = Helper.fontButton;
            for (int i = 0; i < maxPlayers; ++i)
            {
                InputTypeView typeView = new InputTypeView(i, inputTypes[i], font, inputTypeContainer.width, font.FontHeight());
                typeView.selectDelegate = selectDelegate;
                inputTypeContainer.AddView(typeView);
            }
            inputTypeContainer.LayoutVer(0);
            inputTypeContainer.ResizeToFitViewsHor();
            contentView.AddView(inputTypeContainer);

            AddView(contentView);

            // buttons
            View buttons = new View(0.5f * width, contentView.y + contentView.height, 0, 0);
            buttons.alignX = View.ALIGN_CENTER;

            Button button = new TempButton("BACK");
            button.id = (int)ButtonId.Back;
            button.buttonDelegate = buttonDelegate;
            SetCancelButton(button);
            buttons.AddView(button);

            button = new TempButton("START!");
            button.id = (int)ButtonId.Start;
            button.buttonDelegate = buttonDelegate;
            FocusView(button);
            SetConfirmButton(button);
            buttons.AddView(button);

            buttons.LayoutHor(20);
            buttons.ResizeToFitViews();
            AddView(buttons);
        }
    }

    public delegate void InputTypeSelectDelegate(InputTypeView view, bool forward);

    public class InputTypeView : View
    {
        private static IDictionary<InputType, String> inputTypeNameLookup;

        private InputType inputType;
        private TextView typeView;

        public int index;
        public InputTypeSelectDelegate selectDelegate;

        public InputTypeView(int index, InputType inputType, Font font, float width, float height)
            : base(width, height)
        {
            this.index = index;
            this.inputType = inputType;

            focusable = true;

            View leftArrowView = new TextView(font, "<");
            AddView(leftArrowView);

            View rightArrowView = new TextView(font, ">");
            rightArrowView.x = width - rightArrowView.width;
            AddView(rightArrowView);

            typeView = new TextView(font, ToString(inputType));
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
            selectDelegate(this, true);
        }

        private void TrySwitchPrev()
        {
            selectDelegate(this, false);
        }

        protected override void OnFocusChanged(bool focused)
        {
            this.color = focused ? Color.Yellow : Color.White;
        }

        public InputType GetSelectedType()
        {
            return inputType;
        }

        public void SetSelectedType(InputType type)
        {
            inputType = type;
            typeView.SetText(ToString(type));
        }

        private string ToString(InputType type)
        {
            if (inputTypeNameLookup == null)
            {
                inputTypeNameLookup = new Dictionary<InputType, String>();
                inputTypeNameLookup[InputType.None] = "..........";

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
            if (inputTypeNameLookup.TryGetValue(type, out name))
            {
                return name;
            }

            return null;
        }
    }
}
