namespace NIHEI.SC4Buddy.UserFolders.Control
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Plugins.Control;
    using NIHEI.SC4Buddy.Plugins.Installer.FileHandlers;

    public class FolderScanner
    {
        private readonly IPluginFileController pluginFileController;

        public FolderScanner(IPluginFileController pluginFileController, UserFolder userFolder)
        {
            this.pluginFileController = pluginFileController;
            UserFolder = userFolder;
        }

        public UserFolder UserFolder { get; private set; }

        public IEnumerable<string> NewFiles { get; private set; }

        /// <summary>
        /// Scans the folder for new files.
        /// </summary>
        /// <returns>TRUE on new files</returns>
        public bool ScanFolderForNewFiles()
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
