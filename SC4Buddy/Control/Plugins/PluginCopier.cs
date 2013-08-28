namespace NIHEI.SC4Buddy.Control.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities;

    public class PluginCopier
    {
        private readonly PluginController controller;

        public PluginCopier(PluginController controller)
        {
            this.controller = controller;
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

            var files = new List<PluginFile>(plugin.Files.Count);

            files.AddRange(plugin.Files.Select(pluginFile => CopyFile(pluginFile, targetUserFolder)));

            foreach (var pluginFile in files)
            {
                newPlugin.Files.Add(pluginFile);
            }

            controller.Add(plugin);

            if (!moveInsteadOfCopy)
            {
                return;
            }

            var userFolderController = new UserFolderController(RegistryFactory.UserFolderRegistry);
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

            var file = new PluginFile { Checksum = pluginFile.Checksum, Path = newFilePath };
            return file;
        }
    }
}
