namespace NIHEI.SC4Buddy.Control.UserFolders
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.VisualBasic.FileIO;

    using NIHEI.SC4Buddy.Control.Plugins;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Remote;

    public class UserFolderController : IUserFolderController
    {
        private readonly IEntities entities;

        private readonly PluginFileController pluginFileController;

        private readonly PluginController pluginController;

        public UserFolderController(IEntities entities)
        {
            this.entities = entities;

            pluginFileController = new PluginFileController(EntityFactory.Instance.Entities);
            pluginController = new PluginController(EntityFactory.Instance.Entities);
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

        public void SaveChanges()
        {
            entities.SaveChanges();
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
