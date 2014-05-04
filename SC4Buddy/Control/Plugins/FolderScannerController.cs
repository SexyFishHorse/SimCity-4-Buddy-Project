namespace NIHEI.SC4Buddy.Control.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security.Policy;

    using log4net;

    using NIHEI.Common.IO;
    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Remote;

    using RemotePlugin = Irradiated.Sc4Buddy.ApiClient.Model.Plugin;
    using RemotePluginFile = Irradiated.Sc4Buddy.ApiClient.Model.PluginFile;

    public class FolderScannerController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PluginFileController pluginFileController;

        public FolderScannerController(PluginFileController pluginFileController)
        {
            this.pluginFileController = pluginFileController;
        }

        public event EventHandler NewFilesFound;

        public List<string> NewFiles { get; private set; }

        public bool ScanFolder(UserFolder userFolder)
        {
            try
            {
                var folderScanner = new FolderScanner(pluginFileController, userFolder);

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

        public void AutoGroupKnownFiles(
            UserFolder userFolder,
            PluginController pluginController,
            IPluginMatcher pluginMatcher)
        {
            var fileDictionary = GetRemotePluginFileMatches(pluginMatcher, NewFiles);

            var plugins = GroupFilesIntoPlugins(userFolder, fileDictionary, NewFiles);

            foreach (var plugin in plugins.Select(x => x.Value))
            {
                pluginController.Add(plugin, false);
            }

            pluginController.SaveChanges();
        }

        private Dictionary<string, Plugin> GroupFilesIntoPlugins(
            UserFolder userFolder,
            Dictionary<string, RemotePlugin> fileDictionary,
            ICollection<string> allNewFiles)
        {
            var plugins = new Dictionary<string, Plugin>();

            foreach (var remotePlugin in fileDictionary)
            {
                var pluginFile = new PluginFile
                                     {
                                         Checksum = Md5ChecksumUtility.CalculateChecksum(remotePlugin.Key).ToHex(),
                                         Path = remotePlugin.Key
                                     };

                Plugin plugin;
                if (plugins.ContainsKey(remotePlugin.Value.LinkToDownloadPage))
                {
                    plugin = plugins[remotePlugin.Value.LinkToDownloadPage];
                }
                else
                {
                    plugin = new Plugin
                                 {
                                     Author = remotePlugin.Value.AuthorName,
                                     Description = remotePlugin.Value.Description,
                                     Link = new Url(remotePlugin.Value.LinkToDownloadPage),
                                     RemotePlugin = remotePlugin.Value,
                                     UserFolder = userFolder
                                 };

                    plugins.Add(plugin.Link.ToString(), plugin);
                }

                plugin.PluginFiles.Add(pluginFile);

                allNewFiles.Remove(remotePlugin.Key);
            }

            return plugins;
        }

        private Dictionary<string, RemotePlugin> GetRemotePluginFileMatches(
            IPluginMatcher matcher,
            IEnumerable<string> files)
        {
            var fileDictionary = new Dictionary<string, RemotePlugin>();

            foreach (var file in files)
            {
                var match = matcher.GetMostLikelyRemotePluginForFile(
                    file,
                    Md5ChecksumUtility.CalculateChecksum(file).ToHex());

                if (match != null)
                {
                    fileDictionary.Add(file, match);
                }
            }

            return fileDictionary;
        }
    }
}