namespace NIHEI.SC4Buddy.Plugins.Control
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NIHEI.SC4Buddy.Model;

    public class PluginCopier
    {
        private readonly IPluginFileController pluginFileController;

        private readonly IPluginsController pluginsController;

        public PluginCopier(
            IPluginFileController pluginFileController,
            IPluginsController pluginsController)
        {
            this.pluginsController = pluginsController;
            this.pluginFileController = pluginFileController;
        }

        public void CopyPlugin(Plugin plugin, UserFolder targetUserFolder, bool moveInsteadOfCopy = false)
        {
            var newPlugin = new Plugin
                            {
                                Name = plugin.Name,
                                Link = plugin.Link,
                                Description = plugin.Description,
                                Author = plugin.Author,
                                UserFolder = targetUserFolder
                            };

            var files = new List<PluginFile>(plugin.PluginFiles.Count);

            files.AddRange(plugin.PluginFiles.Select(pluginFile => CopyFile(pluginFile, targetUserFolder)));

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

                newPlugin.PluginFiles.Add(pluginFile);
            }

            foreach (var affectedPlugin in affectedPlugins.Where(affectedPlugin => !affectedPlugin.PluginFiles.Any()))
            {
                pluginsController.Remove(affectedPlugin);
            }

            pluginsController.Add(newPlugin);

            if (!moveInsteadOfCopy)
            {
                return;
            }

            pluginsController.UninstallPlugin(plugin);
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

            return new PluginFile { Checksum = pluginFile.Checksum, Path = newFilePath };
        }
    }
}
