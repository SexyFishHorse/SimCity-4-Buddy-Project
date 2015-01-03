namespace NIHEI.SC4Buddy.Application.DataAccess
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using Newtonsoft.Json;

    public class SettingsDataAccess
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public SettingsDataAccess(string storageLocation, string filename)
        {
            DataLocation = Path.Combine(storageLocation, "Configuration", filename);

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
            if (!Settings.Any())
            {
                LoadSettingsFromDisc();
            }

            Settings[key] = value;

            StoreSettingsToDisc();
        }

        public bool HasSetting(string key)
        {
            if (!Settings.Any())
            {
                LoadSettingsFromDisc();
            }

            return Settings.ContainsKey(key);
        }
    }
}
