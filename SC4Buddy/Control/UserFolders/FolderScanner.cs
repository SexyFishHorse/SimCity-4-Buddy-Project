namespace NIHEI.SC4Buddy.Control.UserFolders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NIHEI.SC4Buddy.DataAccess.Plugins;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Installer.FileHandlers;

    public class FolderScanner
    {
        private readonly PluginFileRegistry pluginFileRegistry;

        public FolderScanner(PluginFileRegistry pluginFileRegistry, UserFolder userFolder)
        {
            this.pluginFileRegistry = pluginFileRegistry;
            UserFolder = userFolder;
        }

        public UserFolder UserFolder { get; private set; }

        public IEnumerable<string> NewFiles { get; private set; }

        public bool Run()
        {
            var entries = GetFiles();

            NewFiles = GetNewFiles(entries);

            return NewFiles.Any();
        }

        private IEnumerable<string> GetNewFiles(IEnumerable<string> entries)
        {
            return entries.Where(
                        entry =>
                        !pluginFileRegistry.Files.Any(x => x.Path.Equals(entry, StringComparison.OrdinalIgnoreCase)))
                           .ToList();
        }

        private IEnumerable<string> GetFiles()
        {
            return Directory.EnumerateFiles(Path.Combine(UserFolder.Path, "Plugins"), "*", SearchOption.AllDirectories)
                             .Where(BaseHandler.IsPluginFile);
        }
    }
}
