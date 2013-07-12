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
        Bot,

        Count
    }

    class FooView : View
    {
        private String[] types;
        private bool[] typesDisabled;

        private FooRow[] rows;

        private static IDictionary<InputType, String> inputTypeNameLookup;

        public FooView(int x, int y, int w, int h)
            : base(x, y, w, h)
        {
            Font font = Helper.GetFont(A.fnt_button);

            types = new String[(int)InputType.Count];
            typesDisabled = new bool[(int)InputType.Count];

            for (int i = 0; i < types.Length; ++i)
            {
                types[i] = inputType2Str((InputType)i);
            }

            int maxPlayers = CVars.cg_maxPlayers.intValue;
            rows = new FooRow[maxPlayers];
            for (int i = 0; i < maxPlayers; ++i)
            {
                FooRow row = new FooRow(font, w, font.FontHeight());
                row.id = i;
                row.SetDelegate(OnItemSelected);
                AddView(row);

                rows[i] = row;
            }

            LayoutVer(5);
            ResizeToFitViews();
        }

        private void OnItemSelected(Button button)
        {
            FooRow row = rows[button.id];

            Font font = Helper.GetFont(A.fnt_button);
            PopupList list = new PopupList(font, types, 200);
            list.x = row.x;
            list.y = row.y;
            AddView(list);
        }

        private string inputType2Str(InputType type)
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
            if (inputTypeNameLookup.TryGetValue(type, out name))
            {
                return name;
            }

            return null;
        }
    }

    class FooRow : Button
    {
        private InputType type;

        public FooRow(Font font, int width, int height)
            : base(width, height)
        {   
            focusable = true;

            type = InputType.Undefined;
            TextView typeView = new TextView(font, "......");
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
    }
}
