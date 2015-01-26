namespace NIHEI.SC4Buddy.Plugins.Control
{
    using System;
    using System.IO;
    using NIHEI.SC4Buddy.Model;

    public class PluginCopier
    {
        private readonly IPluginsController pluginsController;

        private readonly IPluginsController targetController;

        public PluginCopier(IPluginsController pluginsController, IPluginsController targetController)
        {
            this.pluginsController = pluginsController;
            this.targetController = targetController;
        }

        public void MovePlugin(Plugin plugin, UserFolder originUserFolder, UserFolder targetUserFolder)
        {
            CopyPlugin(plugin, originUserFolder, targetUserFolder);

            pluginsController.UninstallPlugin(plugin);
        }

        public void CopyPlugin(Plugin plugin, UserFolder originUserFolder, UserFolder targetUserFolder)
        {
            var newPlugin = new Plugin
            {
                Name = plugin.Name,
                Link = plugin.Link,
                Description = plugin.Description,
                Author = plugin.Author
            };

            foreach (var file in plugin.PluginFiles)
            {
                newPlugin.PluginFiles.Add(CopyFile(file, originUserFolder, targetUserFolder));
            }

            targetController.Add(newPlugin);
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
