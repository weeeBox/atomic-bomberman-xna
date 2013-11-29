using System;
using System.IO;

namespace BomberEngine
{
    public class DemoRecorder : IUpdatable, IInputListener, IDestroyable
    {
        private static int BitsPerCmdType = BitUtils.BitsToHoldUInt((int)DemoCmdType.Count);

        private BitWriteBuffer m_buffer;

        private DemoTickCmd m_tickCmd;
        private DemoInputCmd m_inputCmd;

        private static DemoRecorder m_instance;

        public DemoRecorder()
        {
            m_instance = this;

            m_buffer = new BitWriteBuffer();
            m_tickCmd = new DemoTickCmd();
            m_inputCmd = new DemoInputCmd();

            Write(new DemoInitCmd(MathHelp.GetRandomSeed()));
        }

        //////////////////////////////////////////////////////////////////////////////

        #region IUpdatable

        public void Update(float delta)
        {
            if (m_inputCmd.IsChanged)
            {
                Write(m_inputCmd);
            }

            m_tickCmd.frameTime = delta;
            Write(m_tickCmd);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region IDestroyable

        public void Destroy()
        {
            if (m_instance == this)
            {
                m_instance = null;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Commands

        private void Write(DemoCmd cmd)
        {
            m_buffer.Write((uint)cmd.cmdType, BitsPerCmdType);
            cmd.Write(m_buffer);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Save

        public void Save(String path)
        {
            using (Stream stream = FileUtils.OpenWrite(path))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    // version
                    WriteVersion(writer);

                    // storage
                    WriteStorage(writer);

                    // demo data
                    WriteDemo(writer);
                }
            }
        }

        private void WriteVersion(BinaryWriter writer)
        {   
            writer.Write(DemoConstants.Version);
        }

        private void WriteStorage(BinaryWriter writer)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Application.Storage().Save(stream);

                byte[] data = stream.ToArray();
                writer.Write(data.Length);
                writer.Write(data);
            }
        }

        private void WriteDemo(BinaryWriter writer)
        {
            byte[] data = m_buffer.Data;
            int length = m_buffer.LengthBytes;
            int bitLegth = m_buffer.LengthBits;

            writer.Write(bitLegth);
            writer.Write(length);
            writer.Write(data, 0, length);
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
}