namespace NIHEI.SC4Buddy.Control.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.Model;

    public class PluginCopier
    {
        private readonly PluginController pluginController;

        private readonly PluginFileController pluginFileController;

        private readonly UserFolderController userFolderController;

        public PluginCopier(
            PluginController pluginController,
            PluginFileController pluginFileController,
            UserFolderController userFolderController)
        {
            this.pluginController = pluginController;
            this.userFolderController = userFolderController;
            this.pluginFileController = pluginFileController;
        }

        public void CopyPlugin(Plugin plugin, UserFolder targetUserFolder, bool moveInsteadOfCopy = false)
        {
            var newPlugin = new Plugin(Guid.Empty)
                            {
                                Name = plugin.Name,
                                Link = plugin.Link,
                                Description = plugin.Description,
                                Author = plugin.Author,
                                UserFolder = targetUserFolder
                            };

            var files = new List<PluginFile>(plugin.Files.Count);

            files.AddRange(plugin.Files.Select(pluginFile => CopyFile(pluginFile, targetUserFolder)));

            var affectedPlugins = new HashSet<Plugin>();

            foreach (var pluginFile in files)
            {
                if (
                    pluginFileController.Files.Any(
                        x => x.Path.Equals(pluginFile.Path, StringComparison.OrdinalIgnoreCase)))
                {
                    var existingFile =
                        pluginFileController.Files.First(
                            x => x.Path.Equals(pluginFile.Path, StringComparison.OrdinalIgnoreCase));

                    affectedPlugins.Add(existingFile.Plugin);

                    pluginFileController.Delete(existingFile, save: false);
                }

                newPlugin.Files.Add(pluginFile);
            }

            foreach (var affectedPlugin in affectedPlugins.Where(affectedPlugin => !affectedPlugin.Files.Any()))
            {
                pluginController.Delete(affectedPlugin, save: false);
            }

            pluginController.Add(newPlugin, save: false);
            pluginController.SaveChanges();

            if (!moveInsteadOfCopy)
            {
                return;
            }

            userFolderController.UninstallPlugin(plugin);
        }

        private PluginFile CopyFile(PluginFile pluginFile, UserFolder targetUserFolder)
        {
            var currentPath = pluginFile.Path;
            var relativeFilePath = currentPath.Remove(0, pluginFile.Plugin.UserFolder.PluginFolderPath.Length + 1);

            var newFilePath = Path.Combine(targetUserFolder.PluginFolderPath, relativeFilePath);
            var newDirectoryPath = targetUserFolder.PluginFolderPath;
            if (relativeFilePath.Contains("\\"))
            {
                newDirectoryPath = Path.Combine(
                    newDirectoryPath,
                    relativeFilePath.Remove(relativeFilePath.LastIndexOf("\\", StringComparison.OrdinalIgnoreCase)));
            }

            Directory.CreateDirectory(newDirectoryPath);
            File.Copy(currentPath, newFilePath, true);

            return new PluginFile(Guid.Empty) { Checksum = pluginFile.Checksum, Path = newFilePath };
        }
    }
}
