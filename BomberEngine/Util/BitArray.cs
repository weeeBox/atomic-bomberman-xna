using System;

namespace BomberEngine
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
                return BitUtils.GetBit(m_value, index);
            }

            set
            {
                CheckIndex(index);
                m_value = BitUtils.SetBit(m_value, index, value);
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
            set { m_value = value; }
        }

        public int length
        {
            get { return m_length; }
        }
    }
}
