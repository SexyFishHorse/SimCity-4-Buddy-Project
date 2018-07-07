﻿namespace Nihei.SC4Buddy.Plugins.DataAccess
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
    using Nihei.SC4Buddy.Model;
    using Nihei.SC4Buddy.Plugins.Control;
    using Nihei.SC4Buddy.Utils;

    public class PluginsDataAccess
    {
        public const string Filename = "Plugins.json";

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IJsonFileWriter writer;

        private readonly PluginGroupController pluginGroupController;

        public PluginsDataAccess(
            UserFolder userFolder,
            IJsonFileWriter writer,
            PluginGroupController pluginGroupController)
        {
            UserFolder = userFolder;
            this.writer = writer;
            this.pluginGroupController = pluginGroupController;
        }

        public UserFolder UserFolder { get; set; }

        public ICollection<Plugin> LoadPlugins()
        {
            Log.Info(string.Format("Loading plugins for user folder {0}.", UserFolder.FolderPath));

            var path = Path.Combine(UserFolder.PluginFolderPath, Filename);
            var fileInfo = new FileInfo(path);

            var plugins = new Collection<Plugin>();

            if (!fileInfo.Exists)
            {
                return plugins;
            }

            try
            {
                using (var reader = new StreamReader(fileInfo.OpenRead()))
                {
                    var json = reader.ReadToEnd();
                    dynamic pluginsJson = JArray.Parse(json);

                    foreach (var pluginJson in pluginsJson)
                    {
                        var groupName = pluginJson.Group.ToString();
                        var plugin = new Plugin
                        {
                            Id = pluginJson.Id,
                            Author = pluginJson.Author,
                            Description = pluginJson.Description,
                            Name = pluginJson.Name,
                            Link = pluginJson.Link,
                            PluginGroup = pluginGroupController.Groups.FirstOrDefault(x => x.Name == groupName)
                        };

                        if (pluginJson.RemotePluginId != null && pluginJson.RemotePluginId != Guid.Empty.ToString() && !string.IsNullOrWhiteSpace(pluginJson.RemotePluginId.ToString()))
                        {
                            plugin.RemotePlugin = new Asser.Sc4Buddy.Server.Api.V1.Models.Plugin
                            {
                                Id = pluginJson.RemotePluginId
                            };
                        }

                        foreach (var fileJson in pluginJson.PluginFiles)
                        {
                            var file = new PluginFile
                            {
                                Checksum = fileJson.Checksum,
                                Path = fileJson.Path,
                                Id = fileJson.Id,
                                QuarantinedFile = fileJson.QuarantinedFile
                            };
                            plugin.PluginFiles.Add(file);
                        }

                        plugins.Add(plugin);
                    }
                }
            }
            catch (JsonReaderException exception)
            {
                Log.Error(string.Format("Error reading json from {0}", fileInfo.FullName), exception);
            }

            return plugins;
        }

        public void SavePlugins(IEnumerable<Plugin> plugins, UserFolder userFolder)
        {
            Log.Info(string.Format("Save plugins for user folder {0}.", UserFolder.FolderPath));

            var fileInfo = new FileInfo(Path.Combine(userFolder.PluginFolderPath, Filename));

            writer.WriteToFile(fileInfo, plugins);
        }
    }
}
