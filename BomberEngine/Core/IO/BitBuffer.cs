using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.IO
{
    public abstract class BitBuffer
    {
        protected byte[] m_data;
        protected int m_bitLength;

        public virtual void Reset()
        {   
            m_bitLength = 0;
        }

        /// <summary>
        /// Gets or sets the internal data buffer
        /// </summary>
        public byte[] Data
        {
            get { return m_data; }
        }

        /// <summary>
        /// Gets or sets the length of the used portion of the buffer in bytes
        /// </summary>
        public int LengthBytes
        {
            get { return ((m_bitLength + 7) >> 3); }
        }

        /// <summary>
        /// Gets or sets the length of the used portion of the buffer in bits
        /// </summary>
        public int LengthBits
        {
            get { return m_bitLength; }
        }
    }
}
