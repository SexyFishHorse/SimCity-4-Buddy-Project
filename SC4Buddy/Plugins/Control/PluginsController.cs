namespace NIHEI.SC4Buddy.Plugins.Control
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualBasic.FileIO;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Plugins.DataAccess;
    using NIHEI.SC4Buddy.Remote;

    public class PluginsController : IPluginsController
    {
        private readonly IPluginFileController pluginFileController;

        private readonly PluginsDataAccess pluginsDataAccess;

        public PluginsController(IPluginFileController pluginFileController, PluginsDataAccess pluginsDataAccess, UserFolder userFolder)
        {
            this.pluginFileController = pluginFileController;
            this.pluginsDataAccess = pluginsDataAccess;
            UserFolder = userFolder;

            Plugins = pluginsDataAccess.LoadPlugins();
        }

        public ICollection<Plugin> Plugins { get; set; }

        public UserFolder UserFolder { get; set; }

        public static bool IsDamnFile(UserFolder userFolder, string path)
        {
            var relativePath = path.Replace(userFolder.PluginFolderPath, string.Empty);

            return relativePath.StartsWith(@"\DAMN", StringComparison.OrdinalIgnoreCase)
                   && (relativePath.EndsWith("placeholder", StringComparison.OrdinalIgnoreCase)
                       || relativePath.EndsWith("DAMN-Indexer.cmd"));
        }

        public static bool IsBackgroundImage(string entity, UserFolder userFolder)
        {
            if (!userFolder.IsMainFolder)
            {
                return false;
            }

            var validFilenames = new[]
                                     {
                                         "Background3D0.png", "Background3D1.png", "Background3D2.png",
                                         "Background3D3.png", "Background3D4.png"
                                     };

            return validFilenames.Any(entity.EndsWith);
        }

        public void Add(Plugin plugin)
        {
            if (Plugins.Any(x => x.Id == plugin.Id))
            {
                throw new ArgumentException("A plugin with the id " + plugin.Id + " already exists.");
            }

            Plugins.Add(plugin);
            pluginsDataAccess.SavePlugins(Plugins, UserFolder);
        }

        public void Update(Plugin plugin)
        {
            if (Plugins.Any(x => x.Id == plugin.Id))
            {
                Plugins.Remove(plugin);
            }

            Plugins.Add(plugin);

            pluginsDataAccess.SavePlugins(Plugins, UserFolder);
        }

        public void Remove(Plugin plugin)
        {
            if (Plugins.All(x => x.Id != plugin.Id))
            {
                return;
            }

            Plugins.Remove(plugin);
            pluginsDataAccess.SavePlugins(Plugins, UserFolder);
        }

        public void UninstallPlugin(Plugin plugin)
        {
            var files = new PluginFile[plugin.PluginFiles.Count];
            plugin.PluginFiles.CopyTo(files, 0);
            foreach (var file in files)
            {
                if (File.Exists(file.Path))
                {
                    FileSystem.DeleteFile(file.Path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }

                pluginFileController.Delete(file);
            }

            Remove(plugin);
        }

        public async Task<int> UpdateInfoForAllPluginsFromServer(IPluginMatcher pluginMatcher)
        {
            var count = 0;
            foreach (var plugin in Plugins)
            {
                if (await pluginMatcher.MatchAndUpdateAsync(plugin))
                {
                    count++;
                }
            }

            return count;
        }

        public int NumberOfRecognizedPlugins(UserFolder userFolder)
        {
            return Plugins.Count(x => x.RemotePlugin != null);
        }

        public int RemoveEmptyPlugins()
        {
            var emptyPlugins = Plugins.Where(x => !x.PluginFiles.Any()).ToList();

            var counter = emptyPlugins.Count();

            foreach (var emptyPlugin in emptyPlugins)
            {
                Remove(emptyPlugin);
            }

            return counter;
        }
    }
}
