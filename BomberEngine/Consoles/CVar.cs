using System;

namespace BomberEngine
{
    public enum CFlags
    {   
        Readonly = 1,
        Debug    = 2,
        Hidden   = 4,
        DontSave = 8,
    }

    public class CVar
    {
        protected enum VarType
        {
            String,
            Integer,
            Float
        }

        private String m_name;

        private String m_value;
        private String m_defaultValue;

        private int m_intValue;
        private float m_floatValue;

        private VarType m_type;
        private CFlags m_flags;

        public CVar(String name, int defaultValue, CFlags flags = 0)
            : this(name, StringUtils.ToString(defaultValue), VarType.Integer, flags)
        {   
        }

        public CVar(String name, float defaultValue, CFlags flags = 0)
            : this(name, StringUtils.ToString(defaultValue), VarType.Float, flags)
        {   
        }

        public CVar(String name, String defaultValue, CFlags flags = 0)
            : this(name, defaultValue, VarType.String, flags)
        {
        }

        protected CVar(String name, String defaultValue, VarType type, CFlags flags)
        {
            if (name == null)
            {
                throw new NullReferenceException("Name is null");
            }

            m_name = name;
            m_defaultValue = defaultValue;
            m_type = type;
            m_flags = flags;

            SetValue(defaultValue);
        }

        public void SetValue(String value)
        {
            m_value = value;

            if (m_type != VarType.String)
            {
                TrySetFloat(value);
                TrySetInt(value);
            }
        }

        public void SetValue(int v)
        {
            m_value = StringUtils.ToString(v);
            m_intValue = v;
            m_floatValue = v;
        }

        public void SetValue(float v)
        {
            m_value = StringUtils.ToString(v);
            m_intValue = (int)v;
            m_floatValue = v;
        }

        public void Reset()
        {
            SetValue(m_defaultValue);
        }

        //////////////////////////////////////////////////////////////////////////////

        private bool TrySetInt(String value)
        {
            int result;
            if (int.TryParse(value, out result))
            {
                m_intValue = result;
                return true;
            }

            return false;
        }

        private bool TrySetFloat(String value)
        {
            float result;
            if (float.TryParse(value, out result))
            {
                m_floatValue = result;
                return true;
            }

            return false;
        }

        //////////////////////////////////////////////////////////////////////////////

        public String name
        {
            get { return m_name; }
        }

        public String value
        {
            get { return m_value; }
        }

        public String defaultValue
        {
            get { return m_defaultValue; }
        }

        public int intValue
        {
            get { return m_intValue; }
        }

        public float floatValue
        {
            get { return m_floatValue; }
        }

        public bool boolValue
        {
            get { return m_intValue != 0; }
        }

        public bool IsString()
        {
            return m_type == VarType.String;
        }

        public bool IsInt()
        {
            return m_type == VarType.Integer;
        }

        public bool IsFloat()
        {
            return m_type == VarType.Float;
        }

        public bool IsDefault()
        {
            return m_value == m_defaultValue;
        }

        public bool HasFlag(CFlags flag)
        {
            return (m_flags & flag) != 0;
        }
    }
}
