namespace Nihei.SC4Buddy.Configuration
{
    using System;
    using System.IO;
    using Nihei.SC4Buddy.Application.DataAccess;

    public static class Settings
    {
        private static readonly SettingsDataAccess DataAccess = new SettingsDataAccess(GetDefaultStorageLocation(), "Settings.json");

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

        public static string Get(string key, string defaultValue)
        {
            var value = GetRaw(key);

            return value != null ? value.ToString() : defaultValue;
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

        public static T Get<T>(string key, T defaultValue)
        {
            var value = GetRaw(key);

            if (value != null)
            {
                return (T)value;
            }

            return defaultValue;
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

        public static class Keys
        {
            public const string DetectPlugins = "DetectPlugins";

            public const string AutoRunExecutablesDuringInstallation = "AutoRunExecutablesDuringInstallation";

            public const string AskToRemoveNonPluginFilesAfterInstallation = "AskToRemoveNonPluginFilesAfterInstallation";

            public const string FetchInformationFromRemoteServer = "FetchInformationFromRemoteServer";

            public const string AskForAdditionalInformationAfterInstallation = "AskForAdditionalInformationAfterInstallation";

            public const string AllowCheckForMissingDependencies = "AllowCheckForMissingDependencies";

            public const string Wallpaper = "Wallpaper";

            public const string GameLocation = "GameLocation";

            public const string QuarantinedFiles = "QuarantinedFiles";

            public const string ApiBaseUrl = "ApiBaseUrl";
        }
    }
}
