﻿#if BIE
using BepInEx.Configuration;
using UnityExplorer.Config;

namespace UnityExplorer.Loader.BIE
{
    public class BepInExConfigHandler : ConfigHandler
    {
        private ConfigFile Config => ExplorerBepInPlugin.Instance.Config;

        private const string CTG_NAME = "UnityExplorer";

        public override void Init()
        {
            // Not necessary
        }

        public override void RegisterConfigElement<T>(ConfigElement<T> config)
        {
            ConfigEntry<T> entry = Config.Bind(CTG_NAME, config.Name, config.Value, config.Description);

            entry.SettingChanged += (object o, EventArgs e) =>
            {
                config.Value = entry.Value;
            };
        }

        public override T GetConfigValue<T>(ConfigElement<T> element)
        {
            if (Config.TryGetEntry(CTG_NAME, element.Name, out ConfigEntry<T> configEntry))
                return configEntry.Value;
            else
                throw new Exception("Could not get config entry '" + element.Name + "'");
        }

        public override void SetConfigValue<T>(ConfigElement<T> element, T value)
        {
            if (Config.TryGetEntry(CTG_NAME, element.Name, out ConfigEntry<T> configEntry))
                configEntry.Value = value;
            else
                ExplorerCore.Log("Could not get config entry '" + element.Name + "'");
        }

        public override void LoadConfig()
        {
            foreach (KeyValuePair<string, IConfigElement> entry in ConfigManager.ConfigElements)
            {
                string key = entry.Key;
                ConfigDefinition def = new(CTG_NAME, key);
                if (Config.ContainsKey(def) && Config[def] is ConfigEntryBase configEntry)
                {
                    IConfigElement config = entry.Value;
                    config.BoxedValue = configEntry.BoxedValue;
                }
            }
        }

        public override void SaveConfig()
        {
            Config.Save();
        }
    }
}

#endif