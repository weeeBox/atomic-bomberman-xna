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
        private static Dictionary<Buttons, KeyCode> lookup1;
        private static Dictionary<KeyCode, Buttons> lookup2;

        static KeyCodeHelper()
        {
            lookup1 = new Dictionary<Buttons, KeyCode>();
            lookup1.Add(Buttons.DPadUp, KeyCode.GP_DPadUp);
            lookup1.Add(Buttons.DPadDown, KeyCode.GP_DPadDown);
            lookup1.Add(Buttons.DPadLeft, KeyCode.GP_DPadLeft);
            lookup1.Add(Buttons.DPadRight, KeyCode.GP_DPadRight);
            lookup1.Add(Buttons.Start, KeyCode.GP_Start);
            lookup1.Add(Buttons.Back, KeyCode.GP_Back);
            lookup1.Add(Buttons.LeftStick, KeyCode.GP_LeftStick);
            lookup1.Add(Buttons.RightStick, KeyCode.GP_RightStick);
            lookup1.Add(Buttons.LeftShoulder, KeyCode.GP_LeftShoulder);
            lookup1.Add(Buttons.RightShoulder, KeyCode.GP_RightShoulder);
            lookup1.Add(Buttons.BigButton, KeyCode.GP_BigButton);
            lookup1.Add(Buttons.A, KeyCode.GP_A);
            lookup1.Add(Buttons.B, KeyCode.GP_B);
            lookup1.Add(Buttons.X, KeyCode.GP_X);
            lookup1.Add(Buttons.Y, KeyCode.GP_Y);
            lookup1.Add(Buttons.LeftThumbstickLeft, KeyCode.GP_LeftThumbstickLeft);
            lookup1.Add(Buttons.RightTrigger, KeyCode.GP_RightTrigger);
            lookup1.Add(Buttons.LeftTrigger, KeyCode.GP_LeftTrigger);
            lookup1.Add(Buttons.RightThumbstickUp, KeyCode.GP_RightThumbstickUp);
            lookup1.Add(Buttons.RightThumbstickDown, KeyCode.GP_RightThumbstickDown);
            lookup1.Add(Buttons.RightThumbstickRight, KeyCode.GP_RightThumbstickRight);
            lookup1.Add(Buttons.RightThumbstickLeft, KeyCode.GP_RightThumbstickLeft);
            lookup1.Add(Buttons.LeftThumbstickUp, KeyCode.GP_LeftThumbstickUp);
            lookup1.Add(Buttons.LeftThumbstickDown, KeyCode.GP_LeftThumbstickDown);
            lookup1.Add(Buttons.LeftThumbstickRight, KeyCode.GP_LeftThumbstickRight);

            lookup2 = new Dictionary<KeyCode, Buttons>();
            lookup2.Add(KeyCode.GP_DPadUp, Buttons.DPadUp);
            lookup2.Add(KeyCode.GP_DPadDown, Buttons.DPadDown);
            lookup2.Add(KeyCode.GP_DPadLeft, Buttons.DPadLeft);
            lookup2.Add(KeyCode.GP_DPadRight, Buttons.DPadRight);
            lookup2.Add(KeyCode.GP_Start, Buttons.Start);
            lookup2.Add(KeyCode.GP_Back, Buttons.Back);
            lookup2.Add(KeyCode.GP_LeftStick, Buttons.LeftStick);
            lookup2.Add(KeyCode.GP_RightStick, Buttons.RightStick);
            lookup2.Add(KeyCode.GP_LeftShoulder, Buttons.LeftShoulder);
            lookup2.Add(KeyCode.GP_RightShoulder, Buttons.RightShoulder);
            lookup2.Add(KeyCode.GP_BigButton, Buttons.BigButton);
            lookup2.Add(KeyCode.GP_A, Buttons.A);
            lookup2.Add(KeyCode.GP_B, Buttons.B);
            lookup2.Add(KeyCode.GP_X, Buttons.X);
            lookup2.Add(KeyCode.GP_Y, Buttons.Y);
            lookup2.Add(KeyCode.GP_LeftThumbstickLeft, Buttons.LeftThumbstickLeft);
            lookup2.Add(KeyCode.GP_RightTrigger, Buttons.RightTrigger);
            lookup2.Add(KeyCode.GP_LeftTrigger, Buttons.LeftTrigger);
            lookup2.Add(KeyCode.GP_RightThumbstickUp, Buttons.RightThumbstickUp);
            lookup2.Add(KeyCode.GP_RightThumbstickDown, Buttons.RightThumbstickDown);
            lookup2.Add(KeyCode.GP_RightThumbstickRight, Buttons.RightThumbstickRight);
            lookup2.Add(KeyCode.GP_RightThumbstickLeft, Buttons.RightThumbstickLeft);
            lookup2.Add(KeyCode.GP_LeftThumbstickUp, Buttons.LeftThumbstickUp);
            lookup2.Add(KeyCode.GP_LeftThumbstickDown, Buttons.LeftThumbstickDown);
            lookup2.Add(KeyCode.GP_LeftThumbstickRight, Buttons.LeftThumbstickRight);
        }

        public static KeyCode FromButton(Buttons button)
        {   
            return lookup1[button];
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
            if (lookup2.TryGetValue(code, out button))
            {
                return button;
            }

            return (Buttons)(-1);
        }
    }
}
