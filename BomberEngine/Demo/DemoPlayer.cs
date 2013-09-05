using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.IO;
using System.IO;
using BomberEngine.Debugging;
using BomberEngine.Game;
using BomberEngine.Core.Input;
using BomberEngine.Core;

namespace BomberEngine.Demo
{
    public class DemoPlayer : IUpdatable, IKeyInputListener
    {
        private static int BitsPerCmdType = BitUtils.BitsToHoldUInt((int)DemoCmdType.Count);

        private BitReadBuffer m_buffer;
        private IDictionary<DemoCmdType, DemoCmd> m_cmdLookup;

        private DefaultInputManager m_inputManager;

        private bool m_stepByStep;
        private bool m_shouldRunStep;

        public DemoPlayer(String path)
        {
            m_inputManager = new DefaultInputManager();
            m_inputManager.AddKeyboardListener(this);

            m_cmdLookup = new Dictionary<DemoCmdType, DemoCmd>();
            m_cmdLookup[DemoCmdType.Init] = new DemoInitCmd();
            m_cmdLookup[DemoCmdType.Input] = new DemoInputCmd();
            m_cmdLookup[DemoCmdType.Tick]  = new DemoTickCmd();

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

        private void ReadDemo(BinaryReader reader)
        {
            int bitLength = reader.ReadInt32();
            int length = reader.ReadInt32();
            byte[] data = new byte[length];
            int bytesRead = reader.Read(data, 0, length);
            if (bytesRead != length)
            {
                throw new IOException("Wrong data size: " + bytesRead + " expected: " + length);
            }

            m_buffer = new BitReadBuffer(data, bitLength);
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
            else
            {
                ReadTick();
            }
        }

        private void ReadTick()
        {
            while (m_buffer.BitsAvailable > 0)
            {
                DemoCmd cmd = Read(m_buffer);
                bool shouldStop = cmd.Execute();
                if (shouldStop)
                {
                    break;
                }
            }
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
            }

            return false;
        }

        public bool OnKeyRepeated(KeyEventArg arg)
        {
            return false;
        }

        public bool OnKeyReleased(KeyEventArg arg)
        {
            return false;
        }

        #endregion
    }
}
