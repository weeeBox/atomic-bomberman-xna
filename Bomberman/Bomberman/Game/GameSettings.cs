using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;

namespace Bomberman.Game
{
    public sealed class Settings
    {
        private String name;

        private Settings(String name)
        {
            this.name = name;
        }

        public String GetName()
        {
            return name;
        }

        public static readonly Settings BOMB_INITIAL_COUNT      = new Settings("BOMB_INITIAL_COUNT");
        public static readonly Settings BOMB_EXPLOSION_TIMEOUT  = new Settings("BOMB_EXPLOSION_TIMEOUT");
        public static readonly Settings BOMB_EXPLOSION_LENGTH   = new Settings("BOMB_EXPLOSION_LENGTH");
    }

    public class GameSettings
    {
        private static GameSettings instance;

        private Dictionary<String, Object> settings;

        private GameSettings()
        {
            settings = new Dictionary<String, Object>();
        }

        public static bool GetBool(Settings setting)
        {
            return GetBool(setting, false);
        }

        public static bool GetBool(Settings setting, bool defaultValue)
        {
            return GetType(setting, defaultValue);
        }

        public static int GetInt(Settings setting)
        {
            return GetInt(setting, 0);
        }

        public static int GetInt(Settings setting, int defaultValue)
        {
            return GetType(setting, defaultValue);
        }

        public static float GetFloat(Settings setting)
        {
            return GetFloat(setting, 0.0f);
        }

        public static float GetFloat(Settings setting, float defaultValue)
        {
            return GetType(setting, defaultValue);
        }

        public static String GetString(Settings setting)
        {
            return GetString(setting, null);
        }

        public static String GetString(Settings setting, String defaultValue)
        {
            return GetType(setting, defaultValue);
        }

        public static T GetType<T>(Settings setting, T defaultValue)
        {
            Object value = Get(setting);
            return value != null ? (T)value : defaultValue;
        }

        public static Object Get(Settings setting)
        {
            return GetInstance().GetObject(setting);
        }

        private Object GetObject(Settings setting)
        {
            Debug.CheckArgumentNotNull("setting", setting);
            String name = setting.GetName();
            return settings.ContainsKey(name) ? settings[name] : null;
        }

        private static GameSettings GetInstance()
        {
            if (instance == null)
            {
                instance = new GameSettings();
            }
            return instance;
        }
    }
}
