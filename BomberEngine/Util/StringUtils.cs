using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Util
{
    public class StringUtils
    {
        public static string TryFormat(string format, params object[] args)
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
    }
}
