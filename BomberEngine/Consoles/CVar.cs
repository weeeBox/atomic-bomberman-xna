using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;

namespace BomberEngine.Consoles
{
    public class CVar
    {
        protected enum VarType
        {
            String,
            Integer,
            Float
        }

        public const int READONLY = 1;

        public String name;

        public String value;
        public String defaultValue;

        public int intValue;
        public float floatValue;

        protected VarType type;

        public CVar(String name, int defaultValue, int flags = 0)
            : this(name, StringUtils.ToString(defaultValue), VarType.Integer, flags)
        {   
        }

        public CVar(String name, float defaultValue, int flags = 0)
            : this(name, StringUtils.ToString(defaultValue), VarType.Float, flags)
        {   
        }

        public CVar(String name, String defaultValue, int flags = 0)
            : this(name, defaultValue, VarType.String, flags)
        {
        }

        protected CVar(String name, String defaultValue, VarType type, int flags)
        {
            if (name == null)
            {
                throw new NullReferenceException("Name is null");
            }

            if (defaultValue == null)
            {
                throw new NullReferenceException("Default value is null");
            }

            this.name = name;
            this.defaultValue = defaultValue;
            this.type = type;

            SetValue(defaultValue);
        }

        public void SetValue(String value)
        {
            this.value = value;

            if (type != VarType.String)
            {
                TrySetFloat(value);
                TrySetInt(value);
            }
        }

        public void SetValue(int v)
        {
            value = StringUtils.ToString(v);
            intValue = v;
            floatValue = v;
        }

        public void SetValue(float v)
        {
            value = StringUtils.ToString(v);
            intValue = (int)v;
            floatValue = v;
        }

        public void Reset()
        {
            SetValue(defaultValue);
        }

        //////////////////////////////////////////////////////////////////////////////

        private bool TrySetInt(String value)
        {
            int result;
            if (int.TryParse(value, out result))
            {
                intValue = result;
                return true;
            }

            return false;
        }

        private bool TrySetFloat(String value)
        {
            float result;
            if (float.TryParse(value, out result))
            {
                floatValue = result;
                return true;
            }

            return false;
        }

        //////////////////////////////////////////////////////////////////////////////

        public bool IsString()
        {
            return type == VarType.String;
        }

        public bool IsInt()
        {
            return type == VarType.Integer;
        }

        public bool IsFloat()
        {
            return type == VarType.Float;
        }
    }
}
