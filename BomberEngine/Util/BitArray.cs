using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Util
{
    public class BitArray
    {
        public const int MaxLength = 8 * sizeof(long);

        private long m_value;
        private int m_length;

        public BitArray(int length)
        {
            if (length < 0 || length > MaxLength)
            {
                throw new ArgumentException(String.Format("Length {0} is out of range 0..{1}", length, MaxLength));
            }

            m_length = length;
        }

        public bool this[int index]
        {
            get
            {
                CheckIndex(index);
                return (m_value & (1L << index)) != 0;
            }

            set
            {
                CheckIndex(index);
                if (value)
                {
                    m_value |= 1L << index;
                }
                else
                {
                    m_value &= ~(1L << index);
                }
            }
        }

        private void CheckIndex(int index)
        {
            if (index < 0 || index >= m_length)
            {
                throw new IndexOutOfRangeException(String.Format("Index {0} out of bounds 0..{1}", index, m_length));
            }
        }

        public void Clear()
        {
            m_value = 0;
        }

        public override bool Equals(object obj)
        {
            BitArray other = obj as BitArray;
            return other != null && other.m_length == m_length && other.m_value == m_value;
        }

        public override int GetHashCode()
        {
            return (int)m_value;
        }

        public long value
        {
            get { return m_value; }
        }

        public int length
        {
            get { return m_length; }
        }
    }
}
