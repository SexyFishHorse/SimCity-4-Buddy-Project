namespace NIHEI.SC4Buddy.Control
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NIHEI.SC4Buddy.DataAccess;

    public class Settings
    {
        private static readonly SettingsDataAccess DataAccess = new SettingsDataAccess(GetDefaultStorageLocation());

        public static bool HasSetting(string key)
        {
            if (!DataAccess.Settings.Any())
            {
                DataAccess.LoadSettingsFromDisc();
            }

            return DataAccess.Settings.ContainsKey(key);
        }

        public static string Get(string key)
        {
            if (!HasSetting(key))
            {
                throw new KeyNotFoundException(string.Format("No settings for key {0}", key));
            }

            return (string)DataAccess.Settings[key];
        }

        public static T Get<T>(string key)
        {
            if (!HasSetting(key))
            {
                throw new KeyNotFoundException(string.Format("No settings for key {0}", key));
            }

            return (T)DataAccess.Settings[key];
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
            public const string QuarantinedFiles = "QuarantinedFiles";
        }
    }
}
