namespace NIHEI.SC4Buddy.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class SettingsDataAccess
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public SettingsDataAccess(string storageLocation)
        {
            DataLocation = Path.Combine(storageLocation, "Configuration", "Settings.json");

            Settings = new Dictionary<string, object>();
        }

        public IDictionary<string, object> Settings { get; private set; }

        public string DataLocation { get; set; }

        public void LoadSettingsFromDisc()
        {
            if (!File.Exists(DataLocation))
            {
                return;
            }

            using (var reader = new StreamReader(DataLocation))
            {
                var json = reader.ReadToEnd();

                Settings = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
            }
        }

        public void StoreSettingsToDisc()
        {
            if (!Settings.Any())
            {
                Log.Info(string.Format("Empty collection for {0}, skipping storage.", DataLocation));
                return;
            }

            var fileInfo = new FileInfo(DataLocation);

            if (fileInfo.DirectoryName == null)
            {
                throw new DirectoryNotFoundException(string.Format("The location string {0} does not contain a directory name.", DataLocation));
            }

            Directory.CreateDirectory(fileInfo.DirectoryName);

            using (var writer = new StreamWriter(DataLocation))
            {
                writer.Write(JsonConvert.SerializeObject(Settings));
            }
        }

        public void SetSetting(string key, object value)
        {
            Settings[key] = value;

            StoreSettingsToDisc();
        }
    }
}
