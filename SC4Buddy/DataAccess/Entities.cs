namespace Nihei.SC4Buddy.DataAccess
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Nihei.SC4Buddy.Model;

    public class Entities : IEntities
    {
        private const string PluginGroupsFilename = "PluginGroups.json";

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Entities(string storageLocation)
        {
            StorageLocation = storageLocation;
        }

        public ICollection<PluginGroup> Groups { get; private set; }

        private string StorageLocation { get; set; }

        private string PluginGroupsLocation
        {
            get
            {
                return Path.Combine(StorageLocation, PluginGroupsFilename);
            }
        }

        public void SaveChanges()
        {
            StoreDataInFile(Groups, PluginGroupsLocation);
        }

        public void RevertChanges(ModelBase entityObject)
        {
        }

        public void RevertChanges(IEnumerable<ModelBase> entityCollection)
        {
        }

        public void Dispose()
        {
        }

        public void LoadAllEntitiesFromDisc()
        {
            Groups = new Collection<PluginGroup>();

            LoadPluginGroups(PluginGroupsLocation);
        }

        private static void StoreDataInFile(IEnumerable<ModelBase> data, string dataLocation)
        {
            if (!data.Any())
            {
                Log.Info(string.Format("Empty collection for {0}, skipping storage.", dataLocation));
                return;
            }

            var fileInfo = new FileInfo(dataLocation);
            if (fileInfo.DirectoryName == null)
            {
                throw new DirectoryNotFoundException(string.Format("The location string {0} does not contain a directory name.", dataLocation));
            }

            Directory.CreateDirectory(fileInfo.DirectoryName);

            using (var writer = new StreamWriter(dataLocation))
            {
                var json = JsonConvert.SerializeObject(data);
                writer.Write(json);
            }
        }

        private void LoadPluginGroups(string fileLocation)
        {
            if (!File.Exists(fileLocation))
            {
                return;
            }

            using (var reader = new StreamReader(fileLocation))
            {
                var json = reader.ReadToEnd();

                dynamic dynamicGroups = JArray.Parse(json);

                foreach (var dynamicGroup in dynamicGroups)
                {
                    var pluginGroup = new PluginGroup { Id = dynamicGroup.Id, Name = dynamicGroup.Name };
                    var groupPlugins = new Collection<Plugin>();

                    foreach (var pluginId in dynamicGroup.PluginIds)
                    {
                        groupPlugins.Add(new Plugin { Id = pluginId });
                    }

                    pluginGroup.Plugins = groupPlugins;
                    Groups.Add(pluginGroup);
                }
            }
        }
    }
}
