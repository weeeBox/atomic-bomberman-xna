using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;

namespace BomberEngine.Consoles
{
    public class CVar
    {
        public String name;

        public String value;
        public String defaultValue;

        public int valInt;
        public float valFloat;

        public CVar(String name, int defaultValue)
            : this(name, StringUtils.ToString(defaultValue))
        {   
        }

        public CVar(String name, float defaultValue)
            : this(name, StringUtils.ToString(defaultValue))
        {
        }

        public CVar(String name, String defaultValue)
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

            SetValue(defaultValue);
        }

        public void SetValue(String value)
        {
            this.value = value;
            TrySetFloat(value);
            TrySetInt(value);
        }

        public void SetValue(int v)
        {
            value = StringUtils.ToString(v);
            valInt = v;
            valFloat = v;
        }

        public void SetValue(float v)
        {
            value = StringUtils.ToString(v);
            valInt = (int)v;
            valFloat = v;
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
                valInt = result;
                return true;
            }

            return false;
        }

        private bool TrySetFloat(String value)
        {
            float result;
            if (float.TryParse(value, out result))
            {
                valFloat = result;
                return true;
            }

            return false;
        }
    }
}
