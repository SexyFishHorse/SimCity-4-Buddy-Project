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

            var fileDictionary = GetRemotePluginFileMatches(matcher, NewFiles);

            var plugins = new Dictionary<string, Plugin>();

            foreach (var remotePlugin in fileDictionary)
            {
                NewFiles.Remove(remotePlugin.Key);

                var pluginFile = new PluginFile
                                     {
                                         Checksum =
                                             Md5ChecksumUtility.CalculateChecksum(remotePlugin.Key).ToHex(),
                                         Path = remotePlugin.Key
                                     };

                Plugin plugin;
                if (plugins.ContainsKey(remotePlugin.Value.Link))
                {
                    plugin = plugins[remotePlugin.Value.Link];
                }
                else
                {
                    plugin = new Plugin
                                     {
                                         Author = remotePlugin.Value.Author.Name,
                                         Description = remotePlugin.Value.Description,
                                         Link = remotePlugin.Value.Link,
                                         RemotePluginId = remotePlugin.Value.Id,
                                         UserFolder = userFolder
                                     };

                    plugins.Add(plugin.Link, plugin);
                }

                plugin.Files.Add(pluginFile);
            }

            foreach (var plugin in plugins.Select(x => x.Value))
            {
                pluginController.Add(plugin, false);
            }
        }

        private Dictionary<string, RemotePlugin> GetRemotePluginFileMatches(
            PluginMatcher matcher,
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