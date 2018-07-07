namespace Nihei.SC4Buddy.UserFolders.Control
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Nihei.SC4Buddy.Model;
    using Nihei.SC4Buddy.Plugins.Installer.FileHandlers;

    public class FolderScanner
    {
        public FolderScanner(UserFolder userFolder)
        {
            UserFolder = userFolder;
        }

        public UserFolder UserFolder { get; private set; }

        public IEnumerable<string> NewFiles { get; private set; }

        public bool ScanFolderForNewFiles()
        {
            var entries = GetFiles();

            NewFiles = GetNewFiles(entries);

            return NewFiles.Any();
        }

        private IEnumerable<string> GetNewFiles(IEnumerable<string> entries)
        {
            return entries.Where(entry => !UserFolder.Plugins.Any(plugin => plugin.PluginFiles.Any(file => file.Path == entry)));
        }

        private IEnumerable<string> GetFiles()
        {
            return Directory.EnumerateFiles(UserFolder.PluginFolderPath, "*", SearchOption.AllDirectories)
                    .Where(BaseHandler.IsPluginFile);
        }
    }
}
