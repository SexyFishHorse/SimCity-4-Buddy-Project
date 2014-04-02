namespace NIHEI.SC4Buddy.DataAccess
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json;

    using NIHEI.SC4Buddy.Model;

    public class Entities : IEntities
    {
        private readonly string pluginsLocation;

        private readonly string pluginFilesLocation;

        private readonly string groupsLocation;

        private readonly string userFoldersLocation;

        private string StorageLocation { get; set; }

        public Entities(string storageLocation)
        {
            StorageLocation = storageLocation;

            pluginsLocation = Path.Combine(StorageLocation, "Plugins.json");
            pluginFilesLocation = Path.Combine(StorageLocation, "PluginFiles.json");
            groupsLocation = Path.Combine(StorageLocation, "PluginGroups.json");
            userFoldersLocation = Path.Combine(StorageLocation, "UserFolders.json");
        }

        public ICollection<Plugin> Plugins { get; private set; }

        public ICollection<PluginFile> Files { get; private set; }

        public ICollection<UserFolder> UserFolders { get; private set; }

        public ICollection<PluginGroup> Groups { get; private set; }

        public void SaveChanges()
        {
            StoreDataInFile(Plugins, pluginsLocation);
            StoreDataInFile(Files, pluginFilesLocation);
            StoreDataInFile(UserFolders, userFoldersLocation);
            StoreDataInFile(Groups, groupsLocation);
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
            var plugins = GetDataFromFile<Plugin>(pluginsLocation);
            var pluginFiles = GetDataFromFile<PluginFile>(pluginFilesLocation);
            var pluginGroups = GetDataFromFile<PluginGroup>(groupsLocation);
            var userFolders = GetDataFromFile<UserFolder>(userFoldersLocation);

            SetReferences(pluginFiles, plugins, pluginGroups, userFolders);

            Plugins = plugins;
            Files = pluginFiles;
            Groups = pluginGroups;
            UserFolders = userFolders;

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

        private static void StoreDataInFile(IEnumerable<ModelBase> data, string dataLocation)
        {
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
    }
}
