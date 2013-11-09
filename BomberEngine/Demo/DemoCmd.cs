using System;
using System.Collections.Generic;

namespace BomberEngine
{
    public enum DemoCmdType
    {
        Init = 0,
        Tick,
        Input,
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
        private long m_frameIndex;

        private bool m_needsUpdate;

        public DemoTickCmd()
            : base(DemoCmdType.Tick)
        {
        }

        public override bool Execute()
        {
            GetApplication().RunUpdate(m_frameTime);
            ++m_frameIndex;
            return true;
        }

        public override void Write(BitWriteBuffer buffer)
        {
            buffer.Write(m_needsUpdate);
            if (m_needsUpdate)
            {
                buffer.Write(m_frameTime);
                m_needsUpdate = false;
            }
        }

        public override void Read(BitReadBuffer buffer)
        {
            bool needsUpdate = buffer.ReadBoolean();
            if (needsUpdate)
            {
                m_frameTime = buffer.ReadFloat();
            }
        }

        public float frameTime
        {
            get { return m_frameTime; }
            set 
            {
                m_needsUpdate = m_frameTime != value;
                m_frameTime = value; 
            }
        }

        public long frameIndex
        {
            get { return m_frameIndex; }
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class DemoInitCmd : DemoCmd
    {
        private int m_randSeed;

        public DemoInitCmd(int randSeed = 0)
            : base(DemoCmdType.Init)
        {
            m_randSeed = randSeed;
        }

        public override bool Execute()
        {
            MathHelp.InitRandom(m_randSeed);
            return false;
        }

        public override void Write(BitWriteBuffer buffer)
        {
            buffer.Write(m_randSeed);
        }

        public override void Read(BitReadBuffer buffer)
        {
            m_randSeed = buffer.ReadInt32();
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class DemoInputCmd : DemoCmd, IInputListener
    {
        private enum KeyState
        {
            Pressed,
            Released,
            Repeated,
            Count
        }

        private struct KeyEntry
        {
            public KeyEventArg arg;
            public KeyState state;

            public KeyEntry(KeyEventArg arg, KeyState state)
            {
                this.arg = arg;
                this.state = state;
            }
        }

        private static readonly int BitsPerKey = BitUtils.BitsToHoldUInt((int)KeyCode.Count);
        private static readonly int BitsPerKeyState = BitUtils.BitsToHoldUInt((int)KeyState.Count);
        private static readonly int BitsPerPlayerIndex = BitUtils.BitsToHoldUInt(4);

        private List<KeyEntry> m_keyEntries;
        
        private bool m_changed;

        public DemoInputCmd()
            : base(DemoCmdType.Input)
        {
            m_keyEntries = new List<KeyEntry>(128);
        }

        public override bool Execute()
        {
            DemoPlayerInputManager im = Input.Manager as DemoPlayerInputManager;
            Assert.IsTrue(im != null);

            // set pressed states
            for (int i = 0; i < m_keyEntries.Count; ++i)
            {
                if (m_keyEntries[i].state == KeyState.Pressed)
                {
                    im.SetKeyPressed(m_keyEntries[i].arg, true);
                }
                else if (m_keyEntries[i].state == KeyState.Released)
                {
                    im.SetKeyPressed(m_keyEntries[i].arg, false);
                }
            }

            // fire events
            for (int i = 0; i < m_keyEntries.Count; ++i)
            {
                switch (m_keyEntries[i].state)
                {
                    case KeyState.Pressed:
                    {   
                        im.FireKeyPressed(m_keyEntries[i].arg);
                        break;
                    }

                    case KeyState.Repeated:
                    {
                        im.FireKeyRepeated(m_keyEntries[i].arg);
                        break;
                    }

                    case KeyState.Released:
                    {
                        im.FireKeyReleased(m_keyEntries[i].arg);
                        break;
                    }

                    default:
                    {
                        Debug.Fail("Unexpected key state: " + m_keyEntries[i].state);
                        break;
                    }
                }
            }

            return false;
        }

        public override void Write(BitWriteBuffer buffer)
        {
            WriteKeyEntries(buffer, m_keyEntries);
            m_changed = false;
        }

        public override void Read(BitReadBuffer buffer)
        {
            ReadKeyEntries(buffer, m_keyEntries);
        }

        private void WriteKeyEntries(BitWriteBuffer buffer, List<KeyEntry> entries)
        {
            bool hasKeys = entries.Count > 0;
            buffer.Write(hasKeys);
            if (hasKeys)
            {
                buffer.Write((byte)entries.Count);
                for (int i = 0; i < entries.Count; ++i)
                {
                    buffer.Write((uint)entries[i].state, BitsPerKeyState);
                    buffer.Write((uint)entries[i].arg.key, BitsPerKey);

                    bool hasPlayerIndex = entries[i].arg.playerIndex != -1;
                    buffer.Write(hasPlayerIndex);
                    if (hasPlayerIndex)
                    {
                        buffer.Write((uint)entries[i].arg.playerIndex, BitsPerPlayerIndex);
                    }
                }

                entries.Clear();
            }
        }

        private void ReadKeyEntries(BitReadBuffer buffer, List<KeyEntry> entries)
        {
            entries.Clear();

            bool hasKeys = buffer.ReadBoolean();
            if (hasKeys)
            {
                int count = buffer.ReadByte();;
                for (int i = 0; i < count; ++i)
                {
                    KeyState state = (KeyState)buffer.ReadUInt32(BitsPerKeyState);
                    KeyCode key = (KeyCode)buffer.ReadUInt32(BitsPerKey);
                    int playerIndex = -1;
                    bool hasPlayerIndex = buffer.ReadBoolean();
                    if (hasPlayerIndex)
                    {
                        playerIndex = (int)buffer.ReadUInt32(BitsPerPlayerIndex);
                    }

                    entries.Add(new KeyEntry(new KeyEventArg(key, playerIndex), state));
                }
            }
        }

        public bool OnKeyPressed(KeyEventArg arg)
        {
            KeyEntry entry = new KeyEntry(arg, KeyState.Pressed);
            AddKeyEntry(ref entry);
            return false;
        }

        public bool OnKeyRepeated(KeyEventArg arg)
        {
            KeyEntry entry = new KeyEntry(arg, KeyState.Repeated);
            AddKeyEntry(ref entry);
            return false;
        }

        public bool OnKeyReleased(KeyEventArg arg)
        {
            KeyEntry entry = new KeyEntry(arg, KeyState.Released);
            AddKeyEntry(ref entry);
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

        private void AddKeyEntry(ref KeyEntry arg)
        {
            m_keyEntries.Add(arg);
            m_changed = true;
        }

        public bool IsChanged
        {
            get { return m_changed; }
        }
    }
}
