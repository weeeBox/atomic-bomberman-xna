using System;
using System.Collections.Generic;
using System.IO;

namespace BomberEngine
{
    public class DemoPlayer : IUpdatable, IKeyInputListener
    {
        private static int BitsPerCmdType = BitUtils.BitsToHoldUInt((int)DemoCmdType.Count);

        private BitReadBuffer m_buffer;
        private BitReadBuffer m_networkBuffer;

        private IDictionary<DemoCmdType, DemoCmd> m_cmdLookup;

        private DefaultInputManager m_inputManager;

        private bool m_stepByStep;
        private bool m_shouldRunStep;

        private float m_skipTime;
        private int m_frameSkip;

        private IDictionary<KeyCode, int> m_frameSkipLookup;

        private DemoTickCmd m_tickCmd;

        public DemoPlayer(String path)
        {
            m_inputManager = new DefaultInputManager();
            m_inputManager.AddKeyboardListener(this);

            m_tickCmd = new DemoTickCmd();

            m_cmdLookup = new Dictionary<DemoCmdType, DemoCmd>();
            m_cmdLookup[DemoCmdType.Init] = new DemoInitCmd();
            m_cmdLookup[DemoCmdType.Input] = new DemoInputCmd();
            m_cmdLookup[DemoCmdType.Tick] = m_tickCmd;

            m_frameSkipLookup = new Dictionary<KeyCode, int>();
            m_frameSkipLookup[KeyCode.D2] = 2;
            m_frameSkipLookup[KeyCode.D3] = 3;
            m_frameSkipLookup[KeyCode.D4] = 4;
            m_frameSkipLookup[KeyCode.D5] = 5;
            m_frameSkipLookup[KeyCode.D6] = 6;
            m_frameSkipLookup[KeyCode.D7] = 7;
            m_frameSkipLookup[KeyCode.D8] = 8;
            m_frameSkipLookup[KeyCode.D9] = 9;
            m_frameSkipLookup[KeyCode.D0] = 10;

            Read(path);
        }

        private void Read(String path)
        {
            using (Stream stream = FileUtils.OpenRead(path))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    // version
                    byte version = reader.ReadByte();
                    if (version != DemoConstants.Version)
                    {
                        throw new IOException("Version is not supported: " + version);
                    }

                    // storage
                    ReadStorage(reader);

                    // cvars
                    ReadCvars(reader);

                    // demo data
                    ReadDemo(reader);
                }
            }
        }

        private void ReadStorage(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            byte[] data = new byte[length];

            int bytesRead = reader.Read(data, 0, length);
            if (bytesRead != length)
            {
                throw new IOException("Wrong data size: " + bytesRead + " expected: " + length);
            }

            using (MemoryStream stream = new MemoryStream(data))
            {
                Application.Storage().Load(stream);
            }
        }

        private void ReadCvars(BinaryReader reader)
        {
            List<CVar> vars = Application.RootController().Console.ListVars();
            int count = reader.ReadInt32();

            if (count != vars.Count)
            {
                throw new IOException("Wrong cvars count: " + count + " expected: " + vars.Count);
            }

            for (int i = 0; i < vars.Count; ++i)
            {
                CVar var = vars[i];
                String name = reader.ReadString();
                String value = reader.ReadString();

                if (var.name != name)
                {
                    throw new IOException("Unexpected var name: '" + name + "' expected: '" + var.name + "'");
                }

                var.SetValue(value);
            }
        }

        private void ReadDemo(BinaryReader reader)
        {
            m_buffer = ReadBuffer(reader);
            m_networkBuffer = ReadBuffer(reader);
        }

        private BitReadBuffer ReadBuffer(BinaryReader reader)
        {
            int bitLength = reader.ReadInt32();
            int length = (bitLength + 7) >> 3;
            byte[] data = new byte[length];
            int bytesRead = reader.Read(data, 0, length);
            if (bytesRead != length)
            {
                throw new IOException("Wrong data size: " + bytesRead + " expected: " + length);
            }

            return new BitReadBuffer(data, bitLength);
        }

        public void Update(float delta)
        {
            m_inputManager.Update(delta);

            if (m_stepByStep)
            {
                if (m_shouldRunStep)
                {
                    ReadTick();
                    m_shouldRunStep = false;
                }
            }
            else if (m_frameSkip > 0)
            {
                int skip = m_frameSkip;
                while (skip > 0)
                {
                    if (!ReadTick()) break;
                    --skip;
                }
            }
            else
            {
                long targetFrame = CVars.d_demoTargetFrame.intValue;
                if (targetFrame > m_tickCmd.frameIndex)
                {
                    while (targetFrame > m_tickCmd.frameIndex)
                    {
                        if (!ReadTick()) break;
                    }

                    m_stepByStep = true;
                }

                while (m_skipTime >= 0)
                {
                    if (!ReadTick()) break;
                    m_skipTime -= Application.frameTime;
                }

                m_skipTime = 0;
            }
        }

        private bool ReadTick()
        {
            while (m_buffer.BitsAvailable > 0)
            {
                DemoCmd cmd = Read(m_buffer);
                bool shouldStop = cmd.Execute();
                if (shouldStop)
                {
                    return true;
                }
            }

            return false;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Commands

        private DemoCmd Read(BitReadBuffer buffer)
        {
            DemoCmdType cmdType = (DemoCmdType)buffer.ReadUInt32(BitsPerCmdType);
            DemoCmd cmd = FindCmd(cmdType);
            if (cmd == null)
            {
                throw new Exception("Unexpected command: " + cmdType);
            }
            cmd.Read(buffer);
            return cmd;
        }

        private DemoCmd FindCmd(DemoCmdType type)
        {
            DemoCmd cmd;
            if (m_cmdLookup.TryGetValue(type, out cmd))
            {
                return cmd;
            }

            return null;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region IKeyListener

        public bool OnKeyPressed(KeyEventArg arg)
        {
            KeyCode key = arg.key;
            if (key == KeyCode.Space)
            {
                m_stepByStep = !m_stepByStep;
            }
            else if (key == KeyCode.Right)
            {
                if (m_stepByStep)
                {
                    m_shouldRunStep = true;
                }
                else
                {
                    bool shiftPressed = m_inputManager.IsShiftPressed();
                    bool ctrlPressed = m_inputManager.IsControlPressed();
                    bool altPressed = m_inputManager.IsAltPressed();

                    if (shiftPressed && ctrlPressed && altPressed)
                    {
                        m_skipTime = 60.0f;
                    }
                    else if (shiftPressed && ctrlPressed)
                    {
                        m_skipTime = 30.0f;
                    }
                    else if (shiftPressed)
                    {
                        m_skipTime = 10.0f;
                    }
                    else if (ctrlPressed)
                    {
                        m_skipTime = 5.0f;
                    }
                    else
                    {
                        m_skipTime = 1.0f;
                    }
                }
            }
            else
            {
                int skip;
                if (m_frameSkipLookup.TryGetValue(key, out skip))
                {
                    m_frameSkip = skip;
                }
            }

            return false;
        }

        public bool OnKeyRepeated(KeyEventArg arg)
        {
            return false;
        }

        public bool OnKeyReleased(KeyEventArg arg)
        {
            if (m_frameSkipLookup.ContainsKey(arg.key))
            {
                m_frameSkip = 0;
                return true;
            }
            
            return false;
        }

        #endregion        
    }
}
