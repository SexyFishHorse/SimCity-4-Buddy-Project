namespace NIHEI.SC4Buddy.Plugins.Control
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualBasic.FileIO;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Remote;

    public class PluginsController : IPluginsController
    {
        private readonly IPluginFileController pluginFileController;

        private readonly IPluginController pluginController;

        public PluginsController(IPluginFileController pluginFileController, IPluginController pluginController)
        {
            this.pluginFileController = pluginFileController;
            this.pluginController = pluginController;
        }

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

        public void UninstallPlugin(Plugin selectedPlugin)
        {
            var files = new PluginFile[selectedPlugin.PluginFiles.Count];
            selectedPlugin.PluginFiles.CopyTo(files, 0);
            foreach (var file in files)
            {
                if (File.Exists(file.Path))
                {
                    FileSystem.DeleteFile(file.Path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }

                pluginFileController.Delete(file);
            }

            pluginController.Delete(selectedPlugin);
        }

        public async Task<int> UpdateInfoForAllPluginsFromServer(IPluginMatcher pluginMatcher)
        {
            var plugins = pluginController.Plugins;

            var count = 0;
            foreach (var plugin in plugins)
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
            return pluginController.Plugins.Count(x => x.RemotePlugin != null && x.UserFolder.Id == userFolder.Id);
        }
    }
}
