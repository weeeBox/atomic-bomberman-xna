using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Input;
using Microsoft.Xna.Framework;
using BomberEngine.Core;
using BomberEngine.Util;
using BomberEngine.Debugging;

namespace BomberEngine.Demo
{
    public class  DemoPlayerInputManager : InputManager, IResettable
    {
        private struct GamePadState
        {
            private static KeyCode KeyMin = KeyCode.GP_DPadUp;
            private static KeyCode KeyMax = KeyCode.GP_LeftThumbstickRight;

            private bool m_connected;
            
            private bool[] m_keyPressedFlags;

            private Vector2 m_rightThumbStick;
            private Vector2 m_leftThumbStick;

            private float m_leftTrigger;
            private float m_rightTrigger;

            public GamePadState(Object dummy = null)
            {   
                m_connected = false;
                m_keyPressedFlags = new bool[KeyMax - KeyMin];
                m_leftThumbStick = m_rightThumbStick = Vector2.Zero;
                m_leftTrigger = m_rightTrigger = 0.0f;
            }

            public void Reset()
            {
                ArrayUtils.Clear(m_keyPressedFlags);
                m_connected = false;
                m_leftThumbStick = m_rightThumbStick = Vector2.Zero;
                m_leftTrigger = m_rightTrigger = 0.0f;
            }

            public bool IsButtonPressed(KeyCode code)
            {
                Debug.Assert(code >= KeyMin && code <= KeyMax);
                int index = code - KeyMin;

                return m_keyPressedFlags[index];
            }

            public void SetButtonPressed(KeyCode code, bool flag)
            {
                Debug.Assert(code >= KeyMin && code <= KeyMax);
                int index = code - KeyMin;

                m_keyPressedFlags[index] = flag;
            }

            public bool IsConnected
            {
                get { return m_connected; }
                set { m_connected = value; }
            }

            public Vector2 RightThumbStick
            {
                get { return m_rightThumbStick; }
                set { m_rightThumbStick = value; }
            }

            public Vector2 LeftThumbStick
            {
                get { return m_leftThumbStick; }
                set { m_leftThumbStick = value; }
            }

            public float LeftTrigger
            {
                get { return m_leftTrigger; }
                set { m_leftTrigger = value; }
            }

            public float RightTrigger
            {
                get { return m_rightTrigger; }
                set { m_rightTrigger = value; }
            }
        }

        private struct KeyBoardState
        {
            private static KeyCode KeyMin = KeyCode.None;
            private static KeyCode KeyMax = KeyCode.OemClear;

            private bool[] m_keyPressedFlags;

            public KeyBoardState(Object dummy = null)
            {
                m_keyPressedFlags = new bool[KeyMax - KeyMin];
            }

            public void Reset()
            {
                ArrayUtils.Clear(m_keyPressedFlags);
            }

            public bool IsKeyPressed(KeyCode code)
            {
                Debug.Assert(code >= KeyMin && code <= KeyMax);
                int index = code - KeyMin;

                return m_keyPressedFlags[index];
            }

            public void SetKeyPressed(KeyCode code, bool flag)
            {
                Debug.Assert(code >= KeyMin && code <= KeyMax);
                int index = code - KeyMin;

                m_keyPressedFlags[index] = flag;
            }
        }

        private GamePadState[] m_gamePads;
        private KeyBoardState m_keyboard;

        public DemoPlayerInputManager()
        {   
            m_gamePads = new GamePadState[MAX_GAMEPADS_COUNT];
            for (int i = 0; i < m_gamePads.Length; ++i)
            {
                m_gamePads[i] = new GamePadState();
            }
            m_keyboard = new KeyBoardState();
        }

        public void Reset()
        {
            for (int i = 0; i < m_gamePads.Length; ++i)
            {
                m_gamePads[i].Reset();
            }
            m_keyboard.Reset();
        }
        
        public override bool IsKeyPressed(KeyCode code)
        {
            return m_keyboard.IsKeyPressed(code);
        }

        public override bool IsButtonPressed(int playerIndex, KeyCode code)
        {
            return m_gamePads[playerIndex].IsButtonPressed(code);
        }

        public override bool IsGamePadConnected(int playerIndex)
        {
            return m_gamePads[playerIndex].IsConnected;
        }

        public override Vector2 LeftThumbStick(int playerIndex = 0)
        {
            return m_gamePads[playerIndex].LeftThumbStick;
        }

        public override Vector2 RightThumbStick(int playerIndex = 0)
        {
            return m_gamePads[playerIndex].RightThumbStick;
        }

        public override float LeftTrigger(int playerIndex = 0)
        {
            return m_gamePads[playerIndex].LeftTrigger;
        }

        public override float RightTrigger(int playerIndex = 0)
        {
            return m_gamePads[playerIndex].RightTrigger;
        }
    }
}
