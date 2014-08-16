﻿namespace NIHEI.SC4Buddy.Configuration
{
    using System;
    using System.IO;
    using NIHEI.SC4Buddy.DataAccess;

    public class LauncherSettings
    {
        private static readonly SettingsDataAccess DataAccess = new SettingsDataAccess(GetDefaultStorageLocation(), "LauncherSettings.json");

        public static bool HasSetting(string key)
        {
            return DataAccess.HasSetting(key);
        }

        public static string Get(string key)
        {
            if (HasSetting(key))
            {
                return (string)DataAccess.Settings[key];
            }

            return string.Empty;
        }

        public static T Get<T>(string key)
        {
            if (HasSetting(key))
            {
                return (T)DataAccess.Settings[key];
            }

            return default(T);
        }

        public static void SetAndSave(string key, object value)
        {
            DataAccess.SetSetting(key, value);
        }

        public static string GetDefaultStorageLocation()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var storageLocation = Path.Combine(localAppData, "Irradiated Games", "SimCity 4 Buddy");

            return storageLocation;
        }

        public class Keys
        {
            public const string EnableAutoSave = "EnableAutoSave";

            public const string AutoSaveWaitTime = "AutoSaveWaitTime";

            public const string DisableAudio = "DisableAudio";

            public const string DisableMusic = "DisableMusic";

            public const string DisableSounds = "DisableSounds";

            public const string EnableCustomResolution = "EnableCustomResolution";

            public const string WindowMode = "WindowMode";
        }
    }
}