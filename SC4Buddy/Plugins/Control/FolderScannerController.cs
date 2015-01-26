namespace NIHEI.SC4Buddy.Plugins.Control
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Policy;
    using System.Threading.Tasks;
    using Irradiated.Sc4Buddy.ApiClient.Model;
    using log4net;
    using NIHEI.Common.IO;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Remote;
    using NIHEI.SC4Buddy.UserFolders.Control;
    using Plugin = NIHEI.SC4Buddy.Model.Plugin;
    using PluginFile = NIHEI.SC4Buddy.Model.PluginFile;

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

        public async Task<bool> AutoGroupKnownFiles(
            UserFolder userFolder,
            IPluginsController pluginController,
            IPluginMatcher pluginMatcher)
        {
            var fileDictionary = await GetRemotePluginFileMatches(pluginMatcher, NewFiles);

            var plugins = GroupFilesIntoPlugins(fileDictionary, NewFiles);

            foreach (var plugin in plugins)
            {
                pluginController.Add(plugin);
            }

            return true;
        }

        private IEnumerable<Plugin> GroupFilesIntoPlugins(
            Dictionary<string, Irradiated.Sc4Buddy.ApiClient.Model.Plugin> fileDictionary,
            ICollection<string> allNewFiles)
        {
            var plugins = new Collection<Plugin>();
            var filesInPlugins = fileDictionary.GroupBy(x => x.Value).ToList();

            foreach (var pluginGroup in filesInPlugins.Where(x => x.Key != null))
            {
                var remotePlugin = pluginGroup.Key;

                var plugin = new Plugin
                {
                    Id = Guid.NewGuid(),
                    Name = remotePlugin.Name,
                    Description = remotePlugin.Description,
                    Link = new Url(remotePlugin.LinkToDownloadPage),
                    Author = remotePlugin.AuthorName,
                    RemotePlugin = remotePlugin
                };

                var files = new HashSet<PluginFile>();

                foreach (var filePath in pluginGroup.Select(x => x.Key))
                {
                    var pluginFile = new PluginFile
                    {
                        Id = Guid.NewGuid(),
                        Checksum = Md5ChecksumUtility.CalculateChecksum(filePath).ToHex(),
                        Path = filePath,
                        Plugin = plugin
                    };

                    files.Add(pluginFile);
                    allNewFiles.Remove(filePath);
                }

                plugin.PluginFiles = files;
                plugins.Add(plugin);
            }

            return plugins;
        }

        private async Task<Dictionary<string, Irradiated.Sc4Buddy.ApiClient.Model.Plugin>> GetRemotePluginFileMatches(
            IPluginMatcher matcher,
            ICollection<string> files)
        {
            var fileDictionary = new Dictionary<string, Irradiated.Sc4Buddy.ApiClient.Model.Plugin>();

            var pluginFileMetaInfos = new Collection<PluginFileMetaInfo>();

            foreach (
                var tuple in
                    files.Select(file => new FileInfo(file))
                        .Select(
                            fileInfo =>
                            new PluginFileMetaInfo
                            {
                                Filename = fileInfo.Name,
                                Checksum = Md5ChecksumUtility.CalculateChecksum(fileInfo).ToHex()
                            }))
            {
                pluginFileMetaInfos.Add(tuple);
            }

            var matches = await matcher.GetMostLikelyRemotePluginsForFilesAsync(pluginFileMetaInfos);

            foreach (var match in matches)
            {
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    var key = new PluginFileMetaInfo
                              {
                                  Filename = fileInfo.Name,
                                  Checksum = Md5ChecksumUtility.CalculateChecksum(fileInfo).ToHex()
                              };

                    if (!Equals(key, match.Key))
                    {
                        continue;
                    }

                    fileDictionary.Add(file, match.Value.FirstOrDefault());
                    break;
                }
            }

            return fileDictionary;
        }
    }
}