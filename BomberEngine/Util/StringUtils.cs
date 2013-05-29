using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Util
{
    public class StringUtils
    {
        public static String TryFormat(String format, params Object[] args)
        {
            if (args != null && args.Length > 0)
            {
                try
                {
                    return String.Format(format, args);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while formatting string: " + e.Message);
                }
            }

            return format;
        }

        public static String ToString(int value)
        {
            return value.ToString();
        }

        public static String ToString(float value)
        {
            return value.ToString();
        }

        public static String ToString(bool value)
        {
            return value.ToString();
        }
    }
}
