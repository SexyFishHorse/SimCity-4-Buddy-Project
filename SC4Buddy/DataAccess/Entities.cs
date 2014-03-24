namespace NIHEI.SC4Buddy.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Objects;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json;

    using NIHEI.SC4Buddy.Model;

    public class Entities : IEntities
    {
        private string StorageLocation { get; set; }

        public Entities(string storageLocation)
        {
            StorageLocation = storageLocation;
        }

        public IObjectSet<Plugin> Plugins { get; private set; }

        public IObjectSet<PluginFile> Files { get; private set; }

        public IObjectSet<UserFolder> UserFolders { get; set; }

        public IObjectSet<PluginGroup> Groups { get; set; }

        public void SaveChanges()
        {
            throw new NotImplementedException();
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
            var pluginsLocation = Path.Combine(StorageLocation, "Plugins.json");
            var pluginFilesLocation = Path.Combine(StorageLocation, "PluginFiles.json");
            var groupsLocation = Path.Combine(StorageLocation, "PluginGroups.json");
            var userFoldersLocation = Path.Combine(StorageLocation, "UserFolders.json");

            var plugins = GetDataFromFile<Plugin>(pluginsLocation);
            var pluginFiles = GetDataFromFile<PluginFile>(pluginFilesLocation);
            var pluginGroups = GetDataFromFile<PluginGroup>(groupsLocation);
            var userFolders = GetDataFromFile<UserFolder>(userFoldersLocation);

            SetReferences(pluginFiles, plugins, pluginGroups, userFolders);
        }

        private void SetReferences(
            IEnumerable<PluginFile> pluginFiles,
            ICollection<Plugin> plugins,
            ICollection<PluginGroup> pluginGroups,
            ICollection<UserFolder> userFolders)
        {
            foreach (var pluginFile in pluginFiles)
            {
                var plugin = plugins.First(x => x.Id == pluginFile.PluginId);
                var userFolder = userFolders.First(x => x.Id == plugin.UserFolderId);
                var pluginGroup = pluginGroups.First(x => x.Id == plugin.PluginGroupId);

                plugin.PluginFiles.Add(pluginFile);
                pluginFile.Plugin = plugin;

                if (pluginFile.QuarantinedFile != null)
                {
                    pluginFile.QuarantinedFile.PluginFile = pluginFile;
                }

                if (!userFolder.Plugins.Contains(plugin))
                {
                    userFolder.Plugins.Add(plugin);
                }

                if (plugin.UserFolder == null)
                {
                    plugin.UserFolder = userFolder;
                }

                if (!pluginGroup.Plugins.Contains(plugin))
                {
                    pluginGroup.Plugins.Add(plugin);
                }

                if (plugin.PluginGroup == null)
                {
                    plugin.PluginGroup = pluginGroup;
                }
            }
        }

        private static ICollection<T> GetDataFromFile<T>(string dataLocation)
        {
            ICollection<T> output = new Collection<T>();

            if (File.Exists(dataLocation))
            {
                using (var reader = new StreamReader(dataLocation))
                {
                    var json = reader.ReadToEnd();
                    output = JsonConvert.DeserializeObject<ICollection<T>>(json);
                }
            }

            return output;
        }
    }
}
