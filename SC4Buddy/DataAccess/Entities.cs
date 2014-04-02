namespace NIHEI.SC4Buddy.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Security.Policy;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using NIHEI.SC4Buddy.Entities.Remote;
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
            UserFolders = new Collection<UserFolder>();
            Groups = new Collection<PluginGroup>();
            Plugins = new Collection<Plugin>();
            Files = new Collection<PluginFile>();

            LoadUserFolders();

            LoadPluginGroups();

            LoadPlugins();

            LoadPluginFiles();
        }

        private void LoadPluginFiles()
        {
            using (var reader = new StreamReader(pluginFilesLocation))
            {
                var json = reader.ReadToEnd();

                dynamic dynamicPluginFiles = JArray.Parse(json);

                foreach (var dynamicPluginFile in dynamicPluginFiles)
                {
                    var file = new PluginFile
                                   {
                                       Id = dynamicPluginFile.Id,
                                       Checksum = dynamicPluginFile.Checksum,
                                       Path = dynamicPluginFile.Path,
                                       Plugin = Plugins.First(x => x.Id == (Guid)dynamicPluginFile.PluginId),
                                       QuarantinedFile = dynamicPluginFile.QuarantinedFile
                                   };
                    file.Plugin.PluginFiles.Remove(file);
                    file.Plugin.PluginFiles.Add(file);

                    Files.Add(file);
                }
            }
        }

        private void LoadPlugins()
        {
            using (var reader = new StreamReader(pluginsLocation))
            {
                var json = reader.ReadToEnd();

                dynamic dynamicPlugins = JArray.Parse(json);

                foreach (var dynamicPlugin in dynamicPlugins)
                {
                    var plugin = new Plugin
                                     {
                                         Id = dynamicPlugin.Id,
                                         Author = dynamicPlugin.Author,
                                         Description = dynamicPlugin.Description,
                                         Name = dynamicPlugin.Name,
                                         UserFolder = UserFolders.First(x => x.Id == (Guid)dynamicPlugin.UserFolderId)
                                     };

                    if (dynamicPlugin.Link != null)
                    {
                        string link = dynamicPlugin.Link.Value;
                        plugin.Link = new Url(link);
                    }

                    if (dynamicPlugin.PluginGroupId != Guid.Empty)
                    {
                        plugin.PluginGroup = Groups.First(x => x.Id == (Guid)dynamicPlugin.PluginGroupId);
                    }

                    if (dynamicPlugin.RemotePluginId > 0)
                    {
                        plugin.RemotePlugin = new RemotePlugin { Id = dynamicPlugin.RemotePluginId };
                    }

                    var files = new Collection<PluginFile>();

                    foreach (var pluginFileId in dynamicPlugin.PluginFileIds)
                    {
                        files.Add(new PluginFile { Id = pluginFileId });
                    }

                    plugin.PluginFiles = files;

                    UserFolders.First(x => x.Id == plugin.UserFolderId).Plugins.Remove(plugin);
                    UserFolders.First(x => x.Id == plugin.UserFolderId).Plugins.Add(plugin);

                    Plugins.Add(plugin);

                    if (plugin.PluginGroupId != Guid.Empty)
                    {
                        Groups.First(x => x.Id == plugin.PluginGroupId).Plugins.Remove(plugin);
                        Groups.First(x => x.Id == plugin.PluginGroupId).Plugins.Add(plugin);
                    }
                }
            }
        }

        private void LoadPluginGroups()
        {
            using (var reader = new StreamReader(groupsLocation))
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

        private void LoadUserFolders()
        {
            using (var reader = new StreamReader(userFoldersLocation))
            {
                var json = reader.ReadToEnd();

                dynamic dynamicUserFolders = JArray.Parse(json);

                foreach (var dynamicUserFolder in dynamicUserFolders)
                {
                    var userFolder = new UserFolder
                                         {
                                             Id = dynamicUserFolder.Id,
                                             Alias = dynamicUserFolder.Alias,
                                             FolderPath = dynamicUserFolder.FolderPath,
                                             IsMainFolder = dynamicUserFolder.IsMainFolder
                                         };

                    var pluginIds = new Collection<Plugin>();

                    foreach (var pluginId in dynamicUserFolder.PluginIds)
                    {
                        pluginIds.Add(new Plugin { Id = pluginId });
                    }

                    userFolder.Plugins = pluginIds;

                    UserFolders.Add(userFolder);
                }
            }
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
