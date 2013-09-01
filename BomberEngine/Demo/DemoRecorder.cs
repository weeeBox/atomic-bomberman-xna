using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core;
using BomberEngine.Core.Input;
using BomberEngine.Core.IO;
using BomberEngine.Util;

namespace BomberEngine.Demo
{
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
            using (System.IO.Stream stream = FileUtils.OpenWrite(path))
            {
                byte[] data = m_buffer.Data;
                int length = m_buffer.LengthBytes;
                int bitLegth = m_buffer.LengthBits;

                using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(stream))
                {
                    writer.Write(bitLegth);
                    writer.Write(length);
                    stream.Write(data, 0, length);
                }
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
}