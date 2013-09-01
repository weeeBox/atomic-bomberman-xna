using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.IO;
using Microsoft.Xna.Framework.Input;
using BomberEngine.Core.Input;
using BomberEngine.Game;
using BomberEngine.Debugging;

namespace BomberEngine.Demo
{
    public enum DemoCmdType
    {
        Tick,
        Input,
        Random,
        Count
    }

    public abstract class DemoCmd
    {
        private DemoCmdType m_type;

        protected DemoCmd(DemoCmdType type)
        {
            m_type = type;
        }

        // returns "true" if no more commands should be executed
        public abstract bool Execute();
        public abstract void Write(BitWriteBuffer buffer);
        public abstract void Read(BitReadBuffer buffer);

        public DemoCmdType cmdType
        {
            get { return m_type; }
        }

        protected Application GetApplication()
        {
            return Application.sharedApplication;
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class DemoTickCmd : DemoCmd
    {
        private float m_frameTime;

        public DemoTickCmd()
            : base(DemoCmdType.Tick)
        {
        }

        public override bool Execute()
        {
            GetApplication().RunUpdate(m_frameTime);
            return true;
        }

        public override void Write(BitWriteBuffer buffer)
        {
            buffer.Write(m_frameTime);
        }

        public override void Read(BitReadBuffer buffer)
        {
            m_frameTime = buffer.ReadFloat();
        }

        public float frameTime
        {
            get { return m_frameTime; }
            set { m_frameTime = value; }
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class DemoInputCmd : DemoCmd, IInputListener
    {
        private static readonly int BitsPerKey = BitUtils.BitsToHoldUInt((int)KeyCode.Count);
        private static readonly int BitsPerPlayerIndex = 4;

        private List<KeyEventArg> m_pressedKeys;
        private List<KeyEventArg> m_repeatedKeys;
        private List<KeyEventArg> m_releasedKeys;

        private bool m_changed;

        public DemoInputCmd()
            : base(DemoCmdType.Input)
        {
            m_pressedKeys = new List<KeyEventArg>(64);
            m_repeatedKeys = new List<KeyEventArg>(64);
            m_releasedKeys = new List<KeyEventArg>(64);
        }

        public override bool Execute()
        {
            throw new NotImplementedException();
        }

        public override void Write(BitWriteBuffer buffer)
        {
            WriteKeys(buffer, m_pressedKeys);
            WriteKeys(buffer, m_repeatedKeys);
            WriteKeys(buffer, m_releasedKeys);

            m_changed = false;
        }

        public override void Read(BitReadBuffer buffer)
        {
            ReadKeys(buffer, m_pressedKeys);
            ReadKeys(buffer, m_repeatedKeys);
            ReadKeys(buffer, m_releasedKeys);

            DemoPlayerInputManager im = Application.Input() as DemoPlayerInputManager;
            Debug.Assert(im != null);

            
        }

        private void WriteKeys(BitWriteBuffer buffer, List<KeyEventArg> keys)
        {
            bool hasKeys = keys.Count > 0;
            buffer.Write(hasKeys);
            if (hasKeys)
            {
                buffer.Write((byte)keys.Count);
                for (int i = 0; i < keys.Count; ++i)
                {
                    buffer.Write((int)keys[i].key, BitsPerKey);

                    bool hasPlayerIndex = keys[i].playerIndex != -1;
                    buffer.Write(hasPlayerIndex);
                    if (hasPlayerIndex)
                    {
                        buffer.Write(keys[i].playerIndex, BitsPerPlayerIndex);
                    }
                }

                keys.Clear();
            }
        }

        private void ReadKeys(BitReadBuffer buffer, List<KeyEventArg> keys)
        {
            keys.Clear();

            bool hasKeys = buffer.ReadBoolean();
            if (hasKeys)
            {
                int count = buffer.ReadByte();;
                for (int i = 0; i < count; ++i)
                {
                    KeyCode key = (KeyCode)buffer.ReadInt32(BitsPerKey);
                    int playerIndex = -1;
                    bool hasPlayerIndex = buffer.ReadBoolean();
                    if (hasPlayerIndex)
                    {
                        playerIndex = buffer.ReadInt32(BitsPerPlayerIndex);
                    }

                    keys.Add(new KeyEventArg(key, playerIndex));
                }
            }
        }

        public bool OnKeyPressed(KeyEventArg arg)
        {
            AddKeyArg(arg, m_pressedKeys);
            return false;
        }

        public bool OnKeyRepeated(KeyEventArg arg)
        {
            AddKeyArg(arg, m_repeatedKeys);
            return false;
        }

        public bool OnKeyReleased(KeyEventArg arg)
        {
            AddKeyArg(arg, m_releasedKeys);
            return false;
        }

        public void OnPointerMoved(int x, int y, int fingerId)
        {
            throw new NotImplementedException();
        }

        public void OnPointerPressed(int x, int y, int fingerId)
        {
            throw new NotImplementedException();
        }

        public void OnPointerDragged(int x, int y, int fingerId)
        {
            throw new NotImplementedException();
        }

        public void OnPointerReleased(int x, int y, int fingerId)
        {
            throw new NotImplementedException();
        }

        private void AddKeyArg(KeyEventArg arg, List<KeyEventArg> keys)
        {
            keys.Add(arg);
            m_changed = true;
        }

        public bool IsChanged
        {
            get { return m_changed; }
        }
    }
}
