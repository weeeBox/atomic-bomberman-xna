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

namespace Bomberman.Game.Screens
{
    public class GameLobbyScreen : Screen
    {
        public GameLobbyScreen()
        {
            View contentView = new View(64, 48, 521, 384);

            FooView fooView = new FooView(226, 48, 286, 363);
            contentView.AddView(fooView);

            AddView(contentView);
        }
    }

    class FooView : View
    {
        public FooView(int x, int y, int w, int h)
            : base(x, y, w, h)
        {
            int maxPlayers = CVars.cg_maxPlayers.intValue;
            for (int i = 0; i < maxPlayers; ++i)
            {
                AddView(new FooColumn(60, w, 15));
            }

            LayoutVer(5);
            ResizeToFitViews();
        }
    }

    class FooColumn : View
    {
        private enum State
        {
            Off,
            Open,
            Bot,
        }

        public enum InputType
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
        }

        private State state;
        private InputType type;

        private static IDictionary<State, String> stateNameLookup;
        private static IDictionary<InputType, String> inputTypeNameLookup;

        public FooColumn(int firstWidth, int width, int height)
            : base(width, height)
        {   
            focusable = true;

            state = State.Off;
            type = InputType.Undefined;

            Font font = Helper.GetFont(A.fnt_system);
            TextView stateView = new TextView(font, state2Str(state));
            AddView(stateView);

            TextView typeView = new TextView(font, inputType2Str(InputType.Keyboard1));
            typeView.x = firstWidth;
            typeView.width = width - typeView.x;
            AddView(typeView);
        }

        public override bool HandleEvent(Event evt)
        {
            if (evt.code == Event.KEY)
            {
                KeyEvent keyEvent = evt as KeyEvent;
                if (keyEvent.arg.key == KeyCode.Up || keyEvent.arg.key == KeyCode.Down)
                {
                    return false;
                }
            }

            return base.HandleEvent(evt);
        }

        protected override void OnFocusChanged(bool focused)
        {
            this.color = focused ? Color.Yellow : Color.White;
        }

        private String state2Str(State state)
        {
            if (stateNameLookup == null)
            {
                stateNameLookup = new Dictionary<State, String>();
                stateNameLookup[State.Off]  = "....";
                stateNameLookup[State.Open] = "OPEN";
                stateNameLookup[State.Bot] = "BOT";
            }

            return stateNameLookup[state];
        }

        private string inputType2Str(InputType type)
        {
            if (inputTypeNameLookup == null)
            {
                inputTypeNameLookup = new Dictionary<InputType, String>();
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
