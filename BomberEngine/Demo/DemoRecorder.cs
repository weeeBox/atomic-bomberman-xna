using System;
using System.IO;
using System.Collections.Generic;

namespace BomberEngine
{
    public class DemoRecorder : IUpdatable, IInputListener, IDestroyable
    {
        private static int BitsPerCmdType = BitUtils.BitsToHoldUInt((int)DemoCmdType.Count);

        private BitWriteBuffer m_buffer;

        private BitWriteBuffer m_networkBuffer;
        private BitWriteBuffer m_networkTickBuffer;

        private DemoTickCmd m_tickCmd;
        private DemoInputCmd m_inputCmd;

        private static DemoRecorder m_instance;

        public DemoRecorder()
        {
            m_instance = this;

            m_buffer = new BitWriteBuffer();
            m_networkBuffer = new BitWriteBuffer();
            m_networkTickBuffer = new BitWriteBuffer();

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

                    // write cvars
                    WriteCvars(writer);

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

        private void WriteCvars(BinaryWriter writer)
        {
            List<CVar> vars = Application.RootController().Console.ListVars();
            writer.Write(vars.Count);
            for (int i = 0; i < vars.Count; ++i)
            {
                CVar var = vars[i];
                writer.Write(var.name);
                writer.Write(var.value);
            }
        }

        private void WriteDemo(BinaryWriter writer)
        {
            WriteBuffer(writer, m_buffer);
            WriteBuffer(writer, m_networkBuffer);
        }

        private void WriteBuffer(BinaryWriter writer, BitWriteBuffer buffer)
        {
            byte[] data = buffer.Data;
            int length = buffer.LengthBytes;
            int bitLegth = buffer.LengthBits;

            writer.Write(bitLegth);
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

        //////////////////////////////////////////////////////////////////////////////

        #region Network

        private int m_messagesPerTick;

        public void WriteNetworkTick()
        {
            bool hasMessages = m_messagesPerTick > 0;
            m_networkTickBuffer.Write(hasMessages);

            if (hasMessages)
            {
                m_networkBuffer.Write(m_networkTickBuffer.Data, 0, m_networkTickBuffer.LengthBytes);
                m_messagesPerTick = 0;
                m_networkTickBuffer.Reset();
            }
        }

        public void WritePeerConnected(int connectionIndex)
        {
            m_networkTickBuffer.Write(false); // not a data message
            m_networkTickBuffer.Write(true);  // is connected
            m_networkTickBuffer.Write((byte)connectionIndex);
        }

        public void WritePeerDisconnected(int connectionIndex)
        {
            m_networkTickBuffer.Write(false); // not a data message
            m_networkTickBuffer.Write(false); // not connected
            m_networkTickBuffer.Write((byte)connectionIndex);
        }

        public void WritePeerMessage(int bitsLen, byte[] data)
        {
            m_networkTickBuffer.Write(true); // is a data message
            m_networkTickBuffer.Write(bitsLen);

            int bytesLen = (bitsLen + 7) >> 3;
            m_networkTickBuffer.Write(data, 0, bytesLen);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public static DemoRecorder Instance
        {
            get { return m_instance; }
        }

        #endregion
    }
}