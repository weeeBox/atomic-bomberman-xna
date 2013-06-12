using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.IO
{
    public class BitUtils
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
    }
}
