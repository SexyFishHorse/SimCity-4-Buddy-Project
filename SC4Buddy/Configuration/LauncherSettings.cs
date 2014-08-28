namespace NIHEI.SC4Buddy.Configuration
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

        public static object GetRaw(string key)
        {
            return HasSetting(key) ? DataAccess.Settings[key] : null;
        }

        public static string Get(string key)
        {
            var value = GetRaw(key);

            return value != null ? value.ToString() : string.Empty;
        }

        public static int GetInt(string key)
        {
            var stringValue = Get(key);
            int value;

            return int.TryParse(stringValue, out value) ? value : 0;
        }

        public static T Get<T>(string key)
        {
            var value = GetRaw(key);

            if (value != null)
            {
                return (T)value;
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
            public const string WriteLog = "WriteLog";

            public const string DisableIme = "DisableIme";

            public const string IgnoreMissingModels = "IgnoreMissingModels";

            public const string Language = "Language";

            public const string DisableBackgroundLoader = "DisableBackgroundLoader";

            public const string DisableExceptionHandling = "DisableExceptionHandling";

            public const string PauseWhenMinimized = "PauseWhenMinimized";

            public const string SkipIntro = "SkipIntro";

            public const string CpuPriority = "CpuPriority";

            public const string CpuCount = "CpuCount";

            public const string EnableAutoSave = "EnableAutoSave";

            public const string AutoSaveWaitTime = "AutoSaveWaitTime";

            public const string DisableAudio = "DisableAudio";

            public const string DisableMusic = "DisableMusic";

            public const string DisableSounds = "DisableSounds";

            public const string EnableCustomResolution = "EnableCustomResolution";

            public const string WindowMode = "WindowMode";

            public const string Resolution = "Reslution";

            public const string ResolutionColourDepth = "ResolutionColourDepth";

            public const string RenderMode = "RenderMode";

            public const string ColourDepth32Bit = "ColourDepth32Bit";

            public const string CursorColourDepth = "CursorColourDepth";
        }
    }
}
