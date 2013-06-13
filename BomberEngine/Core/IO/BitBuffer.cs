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
            m_data = null;
            m_bitLength = 0;
        }
    }
}
