namespace NIHEI.SC4Buddy.Plugins.Control
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Remote;
    using NIHEI.SC4Buddy.UserFolders.Control;

    public class FolderScannerController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event EventHandler NewFilesFound;

        public List<string> NewFiles { get; private set; }

        public bool ScanFolder(UserFolder userFolder)
        {
            try
            {
                var folderScanner = new FolderScanner(userFolder);

                if (!folderScanner.ScanFolderForNewFiles())
                {
                    return false;
                }

                NewFiles = folderScanner.NewFiles.ToList();

                NewFilesFound(this, EventArgs.Empty);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Error during folder scan: {0}", ex));

                return false;
            }
        }

        public bool AutoGroupKnownFiles(
            UserFolder userFolder,
            IPluginsController pluginController,
            IPluginMatcher pluginMatcher)
        {
            throw new NotImplementedException();
        }
    }
}