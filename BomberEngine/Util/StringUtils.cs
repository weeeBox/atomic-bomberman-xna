using System;

namespace BomberEngine
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

        //////////////////////////////////////////////////////////////////////////////

        #region Parsing

        public static int ParseInt(String str)
        {
            return ParseInt(str, 0);
        }

        public static int ParseInt(String str, int defValue)
        {   
            if (str != null)
            {
                int value;
                bool succeed = int.TryParse(str, out value);
                return succeed ? value : defValue;
            }

            return defValue;
        }

        public static int ParseInt(String str, OutResult result)
        {   
            if (str != null)
            {
                int value;
                result.succeed = int.TryParse(str, out value);
                return result.succeed ? value : 0;
            }

            result.succeed = false;
            return 0;
        }

        public static float ParseFloat(String str)
        {
            return ParseFloat(str, 0.0f);
        }

        public static float ParseFloat(String str, float defValue)
        {
            if (str != null)
            {
                float value;
                bool succeed = float.TryParse(str, out value);
                return succeed ? value : defValue;
            }

            return defValue;
        }

        public static float ParseFloat(String str, OutResult result)
        {
            if (str != null)
            {
                float value;
                result.succeed = float.TryParse(str, out value);
                return result.succeed ? value : 0.0f;
            }

            result.succeed = false;
            return 0.0f;
        }

        public static bool ParseBool(String str)
        {
            return ParseBool(str, false);
        }

        public static bool ParseBool(String str, bool defValue)
        {
            if (str != null)
            {
                bool value;
                bool succeed = bool.TryParse(str, out value);
                return succeed ? value : defValue;
            }

            return defValue;
        }

        public static bool ParseBool(String str, OutResult result)
        {
            if (str != null)
            {
                bool value;
                result.succeed = bool.TryParse(str, out value);
                return result.succeed ? value : false;
            }

            result.succeed = false;
            return false;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region String representation

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

        #endregion
    }
}
