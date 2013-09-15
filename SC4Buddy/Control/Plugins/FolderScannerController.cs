﻿namespace NIHEI.SC4Buddy.Control.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using log4net;

    using NIHEI.Common.IO;
    using NIHEI.SC4Buddy.Control.Remote;
    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Entities.Remote;
    using NIHEI.SC4Buddy.Remote;

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
            RemotePluginFileController remotePluginFileController)
        {
            var matcher = new PluginMatcher(pluginController, remotePluginFileController);

            var fileDictionary = new Dictionary<string, RemotePlugin>();

            foreach (var file in NewFiles)
            {
                var match = matcher.GetMostLikelyRemotePluginForFile(
                    file,
                    Md5ChecksumUtility.CalculateChecksum(file).ToHex());

                if (match != null)
                {
                    fileDictionary.Add(file, match);
                }
            }

            var plugins = new HashSet<Plugin>();

            foreach (var remotePlugin in fileDictionary)
            {
                NewFiles.Remove(remotePlugin.Key);
                var plugin = new Plugin
                                 {
                                     Author = remotePlugin.Value.Author.Name,
                                     Description = remotePlugin.Value.Description,
                                     Link = remotePlugin.Value.Link,
                                     RemotePluginId = remotePlugin.Value.Id,
                                     UserFolder = userFolder
                                 };
                plugin.Files.Add(
                    new PluginFile
                        {
                            Checksum = Md5ChecksumUtility.CalculateChecksum(remotePlugin.Key).ToHex(),
                            Path = remotePlugin.Key
                        });
                plugin.GetHashCode()
                if(plugins.Contains())
                plugins.Add(plugin);
            }
        }
    }
}