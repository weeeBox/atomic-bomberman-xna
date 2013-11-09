using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BomberEngine
{
    public class SharedStorage : IDestroyable
    {
        private String m_filename;
        private TimerManager m_timerManager;

        private IDictionary<String, Object> m_data;

        private IFormatter m_formatter;

        public SharedStorage(String filename)
            : this(filename, Application.TimerManager())
        {
        }

        public SharedStorage(String filename, TimerManager timerManager)
        {
            m_filename = filename;
            m_timerManager = timerManager;

            if (!Load(filename))
            {
                m_data = new Dictionary<String, Object>();
            }
        }

        public void Set(String key, String value)
        {
            SetObject(key, value);
        }

        public void Set(String key, int value)
        {
            SetObject(key, value);
        }

        public void Set(String key, float value)
        {
            SetObject(key, value);
        }

        public void Set(String key, bool value)
        {
            SetObject(key, value);
        }

        public void Remove(String key)
        {
            bool removed = m_data.Remove(key);
            if (removed)
            {
                ScheduleSave();
            }
        }

        public String GetString(String key, String defaultValue = null)
        {
            String value = GetObject(key) as String;
            Assert.IsTrue(value == null || value is String);
            return value != null ? value : defaultValue;
        }

        public int GetInt(String key, int defaultValue = 0)
        {
            Object value = GetObject(key);
            Assert.IsTrue(value == null || value is Int32);
            return value is Int32 ? (int)value : defaultValue;
        }

        public float GetFloat(String key, float defaultValue = 0.0f)
        {
            Object value = GetObject(key);
            Assert.IsTrue(value == null || value is Single || value is Int32);
            return (value is Single) ? (float)value : (value is Int32 ? (int)value : defaultValue);
        }

        public bool GetBool(String key, bool defaultValue = false)
        {
            Object value = GetObject(key);
            Assert.IsTrue(value == null || value is Boolean);
            return value is Boolean ? (bool)value : defaultValue;
        }

        public void Clear()
        {
            m_data.Clear();
            ScheduleSave();
        }

        public bool ContainsKey(String key)
        {
            return m_data.ContainsKey(key);
        }

        private void SetObject(String key, Object value)
        {
            m_data[key] = value;
            ScheduleSave();
        }

        private Object GetObject(String key)
        {
            Object value;
            if (m_data.TryGetValue(key, out value))
            {
                return value;
            }

            return null;
        }

        public void SaveImmediately()
        {
            Save(m_filename);
            m_timerManager.Cancel(Save);
        }

        private void ScheduleSave()
        {
            m_timerManager.ScheduleOnce(Save);
        }

        private void Save()
        {
            Save(m_filename);
        }

        private void Save(String filename)
        {
            using (Stream stream = FileUtils.OpenWrite(filename))
            {
                Save(stream);
            }
        }

        private bool Load(String filename)
        {
            if (FileUtils.FileExists(filename))
            {
                using (Stream stream = FileUtils.OpenRead(filename))
                {
                    return Load(stream);                    
                }
            }

            return false;
        }

        public void Save(Stream stream)
        {
            Formatter.Serialize(stream, m_data);
        }

        public bool Load(Stream stream)
        {
            m_data = Formatter.Deserialize(stream) as IDictionary<String, Object>;
            return m_data != null;
        }

        public void Destroy()
        {
            Save(m_filename);
            m_timerManager.CancelAll(this);
            m_formatter = null;
        }

        private IFormatter Formatter
        {
            get
            {
                if (m_formatter == null)
                {
                    m_formatter = new BinaryFormatter();
                }
                return m_formatter;
            }
        }
    }
}
