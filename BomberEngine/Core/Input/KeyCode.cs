using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace BomberEngine.Core.Input
{
    public enum KeyCode
    {
        None = 0,

        // Keyboard
        Back = 8,
        Tab = 9,
        Enter = 13,
        Pause = 19,
        CapsLock = 20,
        Kana = 21,
        Kanji = 25,
        Escape = 27,
        ImeConvert = 28,
        ImeNoConvert = 29,
        Space = 32,
        PageUp = 33,
        PageDown = 34,
        End = 35,
        Home = 36,
        Left = 37,
        Up = 38,
        Right = 39,
        Down = 40,
        Select = 41,
        Print = 42,
        Execute = 43,
        PrintScreen = 44,
        Insert = 45,
        Delete = 46,
        Help = 47,
        D0 = 48,
        D1 = 49,
        D2 = 50,
        D3 = 51,
        D4 = 52,
        D5 = 53,
        D6 = 54,
        D7 = 55,
        D8 = 56,
        D9 = 57,
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,
        LeftWindows = 91,
        RightWindows = 92,
        Apps = 93,
        Sleep = 95,
        NumPad0 = 96,
        NumPad1 = 97,
        NumPad2 = 98,
        NumPad3 = 99,
        NumPad4 = 100,
        NumPad5 = 101,
        NumPad6 = 102,
        NumPad7 = 103,
        NumPad8 = 104,
        NumPad9 = 105,
        Multiply = 106,
        Add = 107,
        Separator = 108,
        Subtract = 109,
        Decimal = 110,
        Divide = 111,
        F1 = 112,
        F2 = 113,
        F3 = 114,
        F4 = 115,
        F5 = 116,
        F6 = 117,
        F7 = 118,
        F8 = 119,
        F9 = 120,
        F10 = 121,
        F11 = 122,
        F12 = 123,
        F13 = 124,
        F14 = 125,
        F15 = 126,
        F16 = 127,
        F17 = 128,
        F18 = 129,
        F19 = 130,
        F20 = 131,
        F21 = 132,
        F22 = 133,
        F23 = 134,
        F24 = 135,
        NumLock = 144,
        Scroll = 145,
        LeftShift = 160,
        RightShift = 161,
        LeftControl = 162,
        RightControl = 163,
        LeftAlt = 164,
        RightAlt = 165,
        BrowserBack = 166,
        BrowserForward = 167,
        BrowserRefresh = 168,
        BrowserStop = 169,
        BrowserSearch = 170,
        BrowserFavorites = 171,
        BrowserHome = 172,
        VolumeMute = 173,
        VolumeDown = 174,
        VolumeUp = 175,
        MediaNextTrack = 176,
        MediaPreviousTrack = 177,
        MediaStop = 178,
        MediaPlayPause = 179,
        LaunchMail = 180,
        SelectMedia = 181,
        LaunchApplication1 = 182,
        LaunchApplication2 = 183,
        OemSemicolon = 186,
        OemPlus = 187,
        OemComma = 188,
        OemMinus = 189,
        OemPeriod = 190,
        OemQuestion = 191,
        OemTilde = 192,
        ChatPadGreen = 202,
        ChatPadOrange = 203,
        OemOpenBrackets = 219,
        OemPipe = 220,
        OemCloseBrackets = 221,
        OemQuotes = 222,
        Oem8 = 223,
        OemBackslash = 226,
        ProcessKey = 229,
        OemCopy = 242,
        OemAuto = 243,
        OemEnlW = 244,
        Attn = 246,
        Crsel = 247,
        Exsel = 248,
        EraseEof = 249,
        Play = 250,
        Zoom = 251,
        Pa1 = 253,
        OemClear = 254,

        // Gamepad
        GP_DPadUp = 255,
        GP_DPadDown = 256,
        GP_DPadLeft = 257,
        GP_DPadRight = 258,
        GP_Start = 259,
        GP_Back = 260,
        GP_LeftStick = 261,
        GP_RightStick = 262,
        GP_LeftShoulder = 263,
        GP_RightShoulder = 264,
        GP_BigButton = 265,
        GP_A = 266,
        GP_B = 267,
        GP_X = 268,
        GP_Y = 269,
        GP_LeftThumbstickLeft = 270,
        GP_RightTrigger = 271,
        GP_LeftTrigger = 272,
        GP_RightThumbstickUp = 273,
        GP_RightThumbstickDown = 274,
        GP_RightThumbstickRight = 275,
        GP_RightThumbstickLeft = 276,
        GP_LeftThumbstickUp = 277,
        GP_LeftThumbstickDown = 278,
        GP_LeftThumbstickRight = 279,

        TotalCount = 280
    }

    public class KeyCodeHelper
    {
        private struct GPBinding
        {
            public Buttons button;
            public KeyCode code;

            public GPBinding(Buttons button, KeyCode code)
            {
                this.button = button;
                this.code = code;
            }
        }

        private struct NameBinding
        {
            public String name;
            public KeyCode code;

            public NameBinding(String name, KeyCode code)
            {
                this.name = name;
                this.code = code;
            }
        }

        #region name-keycode bindings

        private static readonly NameBinding[] nameBindings = 
        {
            new NameBinding("TAB", KeyCode.Tab),
            new NameBinding("ENTER", KeyCode.Enter),
            new NameBinding("PAUSE", KeyCode.Pause),
            new NameBinding("CAPSLOCK", KeyCode.CapsLock),
            new NameBinding("ESCAPE", KeyCode.Escape),
            new NameBinding("PAGEUP", KeyCode.PageUp),
            new NameBinding("PAGEDOWN", KeyCode.PageDown),
            new NameBinding("END", KeyCode.End),
            new NameBinding("HOME", KeyCode.Home),
            new NameBinding("LEFT", KeyCode.Left),
            new NameBinding("UP", KeyCode.Up),
            new NameBinding("RIGHT", KeyCode.Right),
            new NameBinding("DOWN", KeyCode.Down),
            new NameBinding("INSERT", KeyCode.Insert),
            new NameBinding("DELETE", KeyCode.Delete),
            new NameBinding("0", KeyCode.D0),
            new NameBinding("1", KeyCode.D1),
            new NameBinding("2", KeyCode.D2),
            new NameBinding("3", KeyCode.D3),
            new NameBinding("4", KeyCode.D4),
            new NameBinding("5", KeyCode.D5),
            new NameBinding("6", KeyCode.D6),
            new NameBinding("7", KeyCode.D7),
            new NameBinding("8", KeyCode.D8),
            new NameBinding("9", KeyCode.D9),
            new NameBinding("A", KeyCode.A),
            new NameBinding("B", KeyCode.B),
            new NameBinding("C", KeyCode.C),
            new NameBinding("D", KeyCode.D),
            new NameBinding("E", KeyCode.E),
            new NameBinding("F", KeyCode.F),
            new NameBinding("G", KeyCode.G),
            new NameBinding("H", KeyCode.H),
            new NameBinding("I", KeyCode.I),
            new NameBinding("J", KeyCode.J),
            new NameBinding("K", KeyCode.K),
            new NameBinding("L", KeyCode.L),
            new NameBinding("M", KeyCode.M),
            new NameBinding("N", KeyCode.N),
            new NameBinding("O", KeyCode.O),
            new NameBinding("P", KeyCode.P),
            new NameBinding("Q", KeyCode.Q),
            new NameBinding("R", KeyCode.R),
            new NameBinding("S", KeyCode.S),
            new NameBinding("T", KeyCode.T),
            new NameBinding("U", KeyCode.U),
            new NameBinding("V", KeyCode.V),
            new NameBinding("W", KeyCode.W),
            new NameBinding("X", KeyCode.X),
            new NameBinding("Y", KeyCode.Y),
            new NameBinding("Z", KeyCode.Z),
            new NameBinding("NUM1", KeyCode.NumPad1),
            new NameBinding("NUM2", KeyCode.NumPad2),
            new NameBinding("NUM3", KeyCode.NumPad3),
            new NameBinding("NUM4", KeyCode.NumPad4),
            new NameBinding("NUM5", KeyCode.NumPad5),
            new NameBinding("NUM6", KeyCode.NumPad6),
            new NameBinding("NUM7", KeyCode.NumPad7),
            new NameBinding("NUM8", KeyCode.NumPad8),
            new NameBinding("NUM9", KeyCode.NumPad9),
            new NameBinding("MUL", KeyCode.Multiply),
            new NameBinding("ADD", KeyCode.Add),
            new NameBinding("SEP", KeyCode.Separator),
            new NameBinding("SUB", KeyCode.Subtract),
            new NameBinding("DEC", KeyCode.Decimal),
            new NameBinding("DIV", KeyCode.Divide),
            new NameBinding("F1", KeyCode.F1),
            new NameBinding("F2", KeyCode.F2),
            new NameBinding("F3", KeyCode.F3),
            new NameBinding("F4", KeyCode.F4),
            new NameBinding("F5", KeyCode.F5),
            new NameBinding("F6", KeyCode.F6),
            new NameBinding("F7", KeyCode.F7),
            new NameBinding("F8", KeyCode.F8),
            new NameBinding("F9", KeyCode.F9),
            new NameBinding("F10", KeyCode.F10),
            new NameBinding("F11", KeyCode.F11),
            new NameBinding("F12", KeyCode.F12),
            new NameBinding("F13", KeyCode.F13),
            new NameBinding("F14", KeyCode.F14),
            new NameBinding("F15", KeyCode.F15),
            new NameBinding("F16", KeyCode.F16),
            new NameBinding("F17", KeyCode.F17),
            new NameBinding("F18", KeyCode.F18),
            new NameBinding("F19", KeyCode.F19),
            new NameBinding("F20", KeyCode.F20),
            new NameBinding("F21", KeyCode.F21),
            new NameBinding("F22", KeyCode.F22),
            new NameBinding("F23", KeyCode.F23),
            new NameBinding("F24", KeyCode.F24),
            new NameBinding("RSHIFT", KeyCode.RightShift),
            new NameBinding("LCTRL", KeyCode.LeftControl),
            new NameBinding("RCTRL", KeyCode.RightControl),
            new NameBinding("LALT", KeyCode.LeftAlt),
            new NameBinding("RALT", KeyCode.RightAlt),
            new NameBinding("PLUS", KeyCode.OemPlus),
            new NameBinding("COMMA", KeyCode.OemComma),
            new NameBinding("MINUS", KeyCode.OemMinus),
            new NameBinding("PERIOD", KeyCode.OemPeriod),
            new NameBinding("QUESTION", KeyCode.OemQuestion),
            new NameBinding("TILDE", KeyCode.OemTilde),
            new NameBinding("OPENBRACKETS", KeyCode.OemOpenBrackets),
            new NameBinding("CLOSEBRACKETS", KeyCode.OemCloseBrackets),
            new NameBinding("QUOTES", KeyCode.OemQuotes),

            new NameBinding("JOY_UP", KeyCode.GP_DPadUp),
            new NameBinding("JOY_DOWN", KeyCode.GP_DPadDown),
            new NameBinding("JOY_LEFT", KeyCode.GP_DPadLeft),
            new NameBinding("JOY_RIGHT", KeyCode.GP_DPadRight),
            new NameBinding("JOY_START", KeyCode.GP_Start),
            new NameBinding("JOY_BACK", KeyCode.GP_Back),
            new NameBinding("JOY_LS", KeyCode.GP_LeftStick),
            new NameBinding("JOY_RS", KeyCode.GP_RightStick),
            new NameBinding("JOY_LB", KeyCode.GP_LeftShoulder),
            new NameBinding("JOY_RB", KeyCode.GP_RightShoulder),
            new NameBinding("JOY_A", KeyCode.GP_A),
            new NameBinding("JOY_B", KeyCode.GP_B),
            new NameBinding("JOY_X", KeyCode.GP_X),
            new NameBinding("JOY_Y", KeyCode.GP_Y),
            new NameBinding("JOY_RT", KeyCode.GP_RightTrigger),
            new NameBinding("JOY_LT", KeyCode.GP_LeftTrigger),
        };
#endregion

        #region button-keycode bindings

        private static readonly GPBinding[] gpBindings =
        {
            new GPBinding(Buttons.DPadUp, KeyCode.GP_DPadUp),
            new GPBinding(Buttons.DPadDown, KeyCode.GP_DPadDown),
            new GPBinding(Buttons.DPadLeft, KeyCode.GP_DPadLeft),
            new GPBinding(Buttons.DPadRight, KeyCode.GP_DPadRight),
            new GPBinding(Buttons.Start, KeyCode.GP_Start),
            new GPBinding(Buttons.Back, KeyCode.GP_Back),
            new GPBinding(Buttons.LeftStick, KeyCode.GP_LeftStick),
            new GPBinding(Buttons.RightStick, KeyCode.GP_RightStick),
            new GPBinding(Buttons.LeftShoulder, KeyCode.GP_LeftShoulder),
            new GPBinding(Buttons.RightShoulder, KeyCode.GP_RightShoulder),
            new GPBinding(Buttons.BigButton, KeyCode.GP_BigButton),
            new GPBinding(Buttons.A, KeyCode.GP_A),
            new GPBinding(Buttons.B, KeyCode.GP_B),
            new GPBinding(Buttons.X, KeyCode.GP_X),
            new GPBinding(Buttons.Y, KeyCode.GP_Y),
            new GPBinding(Buttons.LeftThumbstickLeft, KeyCode.GP_LeftThumbstickLeft),
            new GPBinding(Buttons.RightTrigger, KeyCode.GP_RightTrigger),
            new GPBinding(Buttons.LeftTrigger, KeyCode.GP_LeftTrigger),
            new GPBinding(Buttons.RightThumbstickUp, KeyCode.GP_RightThumbstickUp),
            new GPBinding(Buttons.RightThumbstickDown, KeyCode.GP_RightThumbstickDown),
            new GPBinding(Buttons.RightThumbstickRight, KeyCode.GP_RightThumbstickRight),
            new GPBinding(Buttons.RightThumbstickLeft, KeyCode.GP_RightThumbstickLeft),
            new GPBinding(Buttons.LeftThumbstickUp, KeyCode.GP_LeftThumbstickUp),
            new GPBinding(Buttons.LeftThumbstickDown, KeyCode.GP_LeftThumbstickDown),
            new GPBinding(Buttons.LeftThumbstickRight, KeyCode.GP_LeftThumbstickRight),
        };

        #endregion

        private static Dictionary<Buttons, KeyCode> buttonKeyLookup;
        private static Dictionary<KeyCode, Buttons> keyButtonLookup;
        private static Dictionary<String, KeyCode> nameKeyLookup;
        private static Dictionary<KeyCode, String> keyNameLookup;

        static KeyCodeHelper()
        {
            buttonKeyLookup = new Dictionary<Buttons, KeyCode>(gpBindings.Length);
            keyButtonLookup = new Dictionary<KeyCode, Buttons>(gpBindings.Length);
            for (int i = 0; i < gpBindings.Length; ++i)
            {
                Buttons button = gpBindings[i].button;
                KeyCode code = gpBindings[i].code;
                buttonKeyLookup[button] = code;
                keyButtonLookup[code] = button;
            }

            nameKeyLookup = new Dictionary<String, KeyCode>(nameBindings.Length);
            keyNameLookup = new Dictionary<KeyCode, String>(nameBindings.Length);
            for (int i = 0; i < nameBindings.Length; ++i)
            {
                String name = nameBindings[i].name;
                KeyCode code = nameBindings[i].code;
                nameKeyLookup[name] = code;
                keyNameLookup[code] = name;
            }
        }

        public static KeyCode FromButton(Buttons button)
        {
            return buttonKeyLookup[button];
        }

        public static KeyCode FromKey(Keys key)
        {
            int intValue = (int)(key);
            return (KeyCode)intValue;
        }

        public static Keys ToKey(KeyCode code)
        {
            int intValue = (int)code;
            return (Keys)intValue;
        }

        public static Buttons ToButton(KeyCode code)
        {
            Buttons button;
            if (keyButtonLookup.TryGetValue(code, out button))
            {
                return button;
            }

            return (Buttons)(-1);
        }

        public static KeyCode FromString(String value)
        {
            KeyCode code;
            if (nameKeyLookup.TryGetValue(value, out code))
            {
                return code;
            }

            return KeyCode.None;
        }

        public static String ToString(KeyCode code)
        {
            String value;
            if (keyNameLookup.TryGetValue(code, out value))
            {
                return value;
            }

            return null;
        }
    }
}
