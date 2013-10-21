using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.IO
{
    public sealed class BitUtils
    {
        /// <summary>
        /// Returns how many bits are necessary to hold a certain number
        /// </summary>
        public static int BitsToHoldUInt(uint value)
        {
            int bits = 1;
            while ((value >>= 1) != 0)
                bits++;
            return bits;
        }

        /// <summary>
        /// Returns how many bytes are required to hold a certain number of bits
        /// </summary>
        public static int BytesToHoldBits(int numBits)
        {
            return (numBits + 7) / 8;
        }

        public static bool GetBit(long value, int index)
        {
            return (value & (1L << index)) != 0;
        }

        public static long SetBit(long value, int index, bool flag)
        {
            return flag ? (value | (1L << index)) : 
                          (value & ~(1L << index));
        }

        public static bool GetBit(int value, int index)
        {
            return (value & (1 << index)) != 0;
        }

        public static int SetBit(int value, int index, bool flag)
        {
            return flag ? (value | (1 << index)) :
                          (value & ~(1 << index));
        }
    }
}
