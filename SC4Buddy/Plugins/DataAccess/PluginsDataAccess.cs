namespace NIHEI.SC4Buddy.Plugins.DataAccess
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Policy;
    using log4net;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Plugins.Control;
    using NIHEI.SC4Buddy.Utils;

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
                            PluginGroup = pluginGroupController.Groups.FirstOrDefault(x => x.Name == groupName)
                        };

                        if (pluginJson.Link != null)
                        {
                            plugin.Link = new Url(pluginJson.Url);
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
