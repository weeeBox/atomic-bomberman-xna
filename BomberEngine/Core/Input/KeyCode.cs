﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace BomberEngine.Core.Input
{
    public enum KeyCode
    {
        GP_DPadUp = -1,
        GP_DPadDown = -2,
        GP_DPadLeft = -4,
        GP_DPadRight = -8,
        GP_Start = -16,
        GP_Back = -32,
        GP_LeftStick = -64,
        GP_RightStick = -128,
        GP_LeftShoulder = -256,
        GP_RightShoulder = -512,
        GP_BigButton = -2048,
        GP_A = -4096,
        GP_B = -8192,
        GP_X = -16384,
        GP_Y = -32768,
        GP_LeftThumbstickLeft = -2097152,
        GP_RightTrigger = -4194304,
        GP_LeftTrigger = -8388608,
        GP_RightThumbstickUp = -16777216,
        GP_RightThumbstickDown = -33554432,
        GP_RightThumbstickRight = -67108864,
        GP_RightThumbstickLeft = -134217728,
        GP_LeftThumbstickUp = -268435456,
        GP_LeftThumbstickDown = -536870912,
        GP_LeftThumbstickRight = -1073741824,

        KB_None = 0,
        KB_Back = 8,
        KB_Tab = 9,
        KB_Enter = 13,
        KB_Pause = 19,
        KB_CapsLock = 20,
        KB_Kana = 21,
        KB_Kanji = 25,
        KB_Escape = 27,
        KB_ImeConvert = 28,
        KB_ImeNoConvert = 29,
        KB_Space = 32,
        KB_PageUp = 33,
        KB_PageDown = 34,
        KB_End = 35,
        KB_Home = 36,
        KB_Left = 37,
        KB_Up = 38,
        KB_Right = 39,
        KB_Down = 40,
        KB_Select = 41,
        KB_Print = 42,
        KB_Execute = 43,
        KB_PrintScreen = 44,
        KB_Insert = 45,
        KB_Delete = 46,
        KB_Help = 47,
        KB_D0 = 48,
        KB_D1 = 49,
        KB_D2 = 50,
        KB_D3 = 51,
        KB_D4 = 52,
        KB_D5 = 53,
        KB_D6 = 54,
        KB_D7 = 55,
        KB_D8 = 56,
        KB_D9 = 57,
        KB_A = 65,
        KB_B = 66,
        KB_C = 67,
        KB_D = 68,
        KB_E = 69,
        KB_F = 70,
        KB_G = 71,
        KB_H = 72,
        KB_I = 73,
        KB_J = 74,
        KB_K = 75,
        KB_L = 76,
        KB_M = 77,
        KB_N = 78,
        KB_O = 79,
        KB_P = 80,
        KB_Q = 81,
        KB_R = 82,
        KB_S = 83,
        KB_T = 84,
        KB_U = 85,
        KB_V = 86,
        KB_W = 87,
        KB_X = 88,
        KB_Y = 89,
        KB_Z = 90,
        KB_LeftWindows = 91,
        KB_RightWindows = 92,
        KB_Apps = 93,
        KB_Sleep = 95,
        KB_NumPad0 = 96,
        KB_NumPad1 = 97,
        KB_NumPad2 = 98,
        KB_NumPad3 = 99,
        KB_NumPad4 = 100,
        KB_NumPad5 = 101,
        KB_NumPad6 = 102,
        KB_NumPad7 = 103,
        KB_NumPad8 = 104,
        KB_NumPad9 = 105,
        KB_Multiply = 106,
        KB_Add = 107,
        KB_Separator = 108,
        KB_Subtract = 109,
        KB_Decimal = 110,
        KB_Divide = 111,
        KB_F1 = 112,
        KB_F2 = 113,
        KB_F3 = 114,
        KB_F4 = 115,
        KB_F5 = 116,
        KB_F6 = 117,
        KB_F7 = 118,
        KB_F8 = 119,
        KB_F9 = 120,
        KB_F10 = 121,
        KB_F11 = 122,
        KB_F12 = 123,
        KB_F13 = 124,
        KB_F14 = 125,
        KB_F15 = 126,
        KB_F16 = 127,
        KB_F17 = 128,
        KB_F18 = 129,
        KB_F19 = 130,
        KB_F20 = 131,
        KB_F21 = 132,
        KB_F22 = 133,
        KB_F23 = 134,
        KB_F24 = 135,
        KB_NumLock = 144,
        KB_Scroll = 145,
        KB_LeftShift = 160,
        KB_RightShift = 161,
        KB_LeftControl = 162,
        KB_RightControl = 163,
        KB_LeftAlt = 164,
        KB_RightAlt = 165,
        KB_BrowserBack = 166,
        KB_BrowserForward = 167,
        KB_BrowserRefresh = 168,
        KB_BrowserStop = 169,
        KB_BrowserSearch = 170,
        KB_BrowserFavorites = 171,
        KB_BrowserHome = 172,
        KB_VolumeMute = 173,
        KB_VolumeDown = 174,
        KB_VolumeUp = 175,
        KB_MediaNextTrack = 176,
        KB_MediaPreviousTrack = 177,
        KB_MediaStop = 178,
        KB_MediaPlayPause = 179,
        KB_LaunchMail = 180,
        KB_SelectMedia = 181,
        KB_LaunchApplication1 = 182,
        KB_LaunchApplication2 = 183,
        KB_OemSemicolon = 186,
        KB_OemPlus = 187,
        KB_OemComma = 188,
        KB_OemMinus = 189,
        KB_OemPeriod = 190,
        KB_OemQuestion = 191,
        KB_OemTilde = 192,
        KB_ChatPadGreen = 202,
        KB_ChatPadOrange = 203,
        KB_OemOpenBrackets = 219,
        KB_OemPipe = 220,
        KB_OemCloseBrackets = 221,
        KB_OemQuotes = 222,
        KB_Oem8 = 223,
        KB_OemBackslash = 226,
        KB_ProcessKey = 229,
        KB_OemCopy = 242,
        KB_OemAuto = 243,
        KB_OemEnlW = 244,
        KB_Attn = 246,
        KB_Crsel = 247,
        KB_Exsel = 248,
        KB_EraseEof = 249,
        KB_Play = 250,
        KB_Zoom = 251,
        KB_Pa1 = 253,
        KB_OemClear = 254,
    }

    public class KeyCodeHelper
    {
        public static KeyCode FromButton(Buttons button)
        {
            int intValue = -(int)(button);
            return (KeyCode)intValue;
        }

        public static KeyCode FromKey(Keys key)
        {
            int intValue = (int)(key);
            return (KeyCode)intValue;
        }
    }
}