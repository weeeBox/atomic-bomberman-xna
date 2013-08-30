using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.IO;
using System.IO;

namespace BomberEngine.Demo
{
    /*
    public class DemoRecorder : IUpdatable, IInputListener
    {
        private static int BitsPerCmdType = BitUtils.BitsToHoldUInt((int)DemoCmdType.Count);

        private BitWriteBuffer m_buffer;

        private DemoTickCmd m_tickCmd;
        private DemoInputCmd m_inputCmd;
        
        public DemoRecorder()
        {
            m_buffer = new BitWriteBuffer();
            m_tickCmd = new DemoTickCmd();
            m_inputCmd = new DemoInputCmd();
        }

        //////////////////////////////////////////////////////////////////////////////

        #region IUpdatable

        public void Update(float delta)
        {
            m_tickCmd.frameTime = delta;
            Write(m_tickCmd);

            if (m_inputCmd.IsChanged)
            {
                Write(m_inputCmd);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Commands

        private void Write(DemoCmd cmd)
        {
            m_buffer.Write((int)cmd.cmdType, BitsPerCmdType);
            cmd.Write(m_buffer);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Save

        public void Save(String path)
        {
            using (System.IO.Stream stream = FileUtils.OpenWrite(path))
            {
                byte[] data = m_buffer.Data;
                int length = m_buffer.LengthBytes;

                stream.Write(data, 0, length);
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region InputListener

        public bool OnKeyPressed(KeyEventArg arg)
        {
            return m_inputCmd.OnKeyPressed(arg);
        }

        public bool OnKeyRepeated(KeyEventArg arg)
        {
            return m_inputCmd.OnKeyRepeated(arg);
        }

        public bool OnKeyReleased(KeyEventArg arg)
        {
            return m_inputCmd.OnKeyReleased(arg);
        }

        public void OnPointerMoved(int x, int y, int fingerId)
        {
            m_inputCmd.OnPointerMoved(x, y, fingerId);
        }

        public void OnPointerPressed(int x, int y, int fingerId)
        {
            m_inputCmd.OnPointerPressed(x, y, fingerId);
        }

        public void OnPointerDragged(int x, int y, int fingerId)
        {
            m_inputCmd.OnPointerDragged(x, y, fingerId);
        }

        public void OnPointerReleased(int x, int y, int fingerId)
        {
            m_inputCmd.OnPointerReleased(x, y, fingerId);
        }

        #endregion
    }
    */

    public class DemoPlayer
    {
        private static int BitsPerCmdType = BitUtils.BitsToHoldUInt((int)DemoCmdType.Count);

        private BitReadBuffer m_buffer;
        private IDictionary<DemoCmdType, DemoCmd> m_cmdLookup;

        public DemoPlayer()
        {   
            m_cmdLookup = new Dictionary<DemoCmdType, DemoCmd>();
            m_cmdLookup[DemoCmdType.Input] = new DemoInputCmd();
            m_cmdLookup[DemoCmdType.Tick]  = new DemoTickCmd();
        }

        public void Read(String path)
        {
            using (Stream stream = FileUtils.OpenRead(path))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int bitLength = reader.ReadInt32();
                    int length = reader.ReadInt32();
                    byte[] data = new byte[length];
                    int bytesRead = reader.Read(data, 0, length);
                    if (bytesRead != length)
                    {
                        throw new IOException("Wrong data size: " + bytesRead + " expected: " + length);
                    }
                }
            }
        }

        public void ReadNextCmd()
        {

        }
    }
}
