namespace NIHEI.SC4Buddy.Control.UserFolders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NIHEI.SC4Buddy.Control.Plugins;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Installer.FileHandlers;

    public class FolderScanner
    {
        private readonly PluginFileController pluginFileController;

        public FolderScanner(PluginFileController pluginFileController, UserFolder userFolder)
        {
            this.pluginFileController = pluginFileController;
            UserFolder = userFolder;
        }

        public UserFolder UserFolder { get; private set; }

        public IEnumerable<string> NewFiles { get; private set; }

        public bool ScanFolder()
        {
            var entries = GetFiles();

            NewFiles = GetNewFiles(entries);

            return NewFiles.Any();
        }

        private IEnumerable<string> GetNewFiles(IEnumerable<string> entries)
        {
            return entries.Where(
                        entry =>
                        !pluginFileController.Files.Any(x => x.Path.Equals(entry, StringComparison.OrdinalIgnoreCase)))
                           .ToList();
        }

        private IEnumerable<string> GetFiles()
        {
            return Directory.EnumerateFiles(UserFolder.PluginFolderPath, "*", SearchOption.AllDirectories)
                             .Where(BaseHandler.IsPluginFile);
        }
    }
}
