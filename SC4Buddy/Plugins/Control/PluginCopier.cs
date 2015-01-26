namespace NIHEI.SC4Buddy.Plugins.Control
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NIHEI.SC4Buddy.Model;

    public class PluginCopier
    {
        private readonly IPluginsController pluginsController;

        public PluginCopier(IPluginsController pluginsController)
        {
            this.pluginsController = pluginsController;
        }

        public void CopyPlugin(Plugin plugin, UserFolder originUserFolder, UserFolder targetUserFolder, bool moveInsteadOfCopy = false)
        {
            var newPlugin = new Plugin
            {
                Name = plugin.Name,
                Link = plugin.Link,
                Description = plugin.Description,
                Author = plugin.Author
            };

            var files = new List<PluginFile>(plugin.PluginFiles.Count);

            files.AddRange(plugin.PluginFiles.Select(pluginFile => CopyFile(pluginFile, originUserFolder, targetUserFolder)));

            var affectedPlugins = new HashSet<Plugin>();

            foreach (var pluginFile in files)
            {
                if (plugin.PluginFiles.Any(x => x.Path.Equals(pluginFile.Path, StringComparison.OrdinalIgnoreCase)))
                {
                    var existingFile = plugin.PluginFiles.First(x => x.Path.Equals(pluginFile.Path, StringComparison.OrdinalIgnoreCase));

                    affectedPlugins.Add(existingFile.Plugin);
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

        private static PluginFile CopyFile(PluginFile pluginFile, UserFolder originUserFolder, UserFolder targetUserFolder)
        {
            var currentPath = pluginFile.Path;
            var relativeFilePath = currentPath.Remove(0, originUserFolder.PluginFolderPath.Length + 1);

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
