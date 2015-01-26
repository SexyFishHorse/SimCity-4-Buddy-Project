namespace NIHEI.SC4Buddy.Plugins.DataAccess
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Reflection;
    using System.Security.Policy;
    using log4net;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NIHEI.SC4Buddy.Model;

    public class PluginsDataAccess
    {
        public const string Filename = "plugins.json";

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public PluginsDataAccess(UserFolder userFolder)
        {
            UserFolder = userFolder;
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

            using (var reader = new StreamReader(fileInfo.OpenRead()))
            {
                var json = reader.ReadToEnd();
                dynamic pluginsJson = JArray.Parse(json);

                foreach (var pluginJson in pluginsJson)
                {
                    var plugin = new Plugin
                    {
                        Author = pluginJson.Author,
                        Id = pluginJson.Id,
                        Description = pluginJson.Description,
                        Name = pluginJson.Name
                    };

                    if (pluginJson.Link != null)
                    {
                        plugin.Link = new Url(pluginJson.Url);
                    }

                    plugins.Add(plugin);
                }

                return plugins;
            }
        }

        public void SavePlugins(IEnumerable<Plugin> plugins, UserFolder userFolder)
        {
            Log.Info(string.Format("Save plugins for user folder {0}.", UserFolder.FolderPath));

            var fileInfo = new FileInfo(Path.Combine(userFolder.PluginFolderPath, Filename));

            if (fileInfo.DirectoryName == null)
            {
                throw new DirectoryNotFoundException(string.Format("The location string {0} does not contain a directory name.", fileInfo.FullName));
            }

            Directory.CreateDirectory(fileInfo.DirectoryName);

            using (var writer = new StreamWriter(fileInfo.OpenWrite()))
            {
                var json = JsonConvert.SerializeObject(plugins);
                writer.Write(json);
            }
        }
    }
}
