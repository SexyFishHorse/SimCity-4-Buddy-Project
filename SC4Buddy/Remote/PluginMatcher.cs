namespace NIHEI.SC4Buddy.Remote
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NIHEI.SC4Buddy.Control.Plugins;
    using NIHEI.SC4Buddy.Control.Remote;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Entities.Remote;

    public class PluginMatcher
    {
        private readonly RemotePluginFileController remotePluginFileController;

        private readonly PluginController pluginController;

        public PluginMatcher(PluginController pluginController, RemotePluginFileController remotePluginFileController)
        {
            this.remotePluginFileController = remotePluginFileController;
            this.pluginController = pluginController;
        }

        public IEnumerable<RemotePlugin> GetPossibleRemotePlugins(IList<PluginFile> files)
        {
            var fileMatches = new List<RemotePluginFile>(files.Count());

            foreach (var file in files)
            {
                fileMatches.AddRange(GetPossibleRemotePluginFilesForFile(file));
            }

            var allPlugins = fileMatches.Select(match => match.Plugin).ToList();

            return
                allPlugins.GroupBy(x => x.Id)
                          .Select(x => new { Plugin = x.First(), Count = x.Count() })
                          .OrderByDescending(x => x.Count)
                          .Select(x => x.Plugin)
                          .ToList();
        }

        public RemotePlugin GetMostLikelyRemotePlugin(IList<PluginFile> files)
        {
            var firstPlugin = GetPossibleRemotePlugins(files).FirstOrDefault();
            return firstPlugin;
        }

        public bool MatchAndUpdate(Plugin plugin)
        {
            var remotePlugin = GetMostLikelyRemotePlugin(plugin.Files.ToList());
            if (remotePlugin == null)
            {
                return false;
            }

            plugin.RemotePluginId = remotePlugin.Id;
            plugin.Name = remotePlugin.Name;
            plugin.Author = remotePlugin.Author.Name;
            plugin.Link = remotePlugin.Link;
            plugin.Description = remotePlugin.Description;

            pluginController.SaveChanges();

            return true;
        }

        public RemotePlugin GetMostLikelyRemotePluginForFile(PluginFile file)
        {
            var matches = GetPossibleRemotePluginFilesForFile(file);
            var remotePluginFile = matches.FirstOrDefault();

            return remotePluginFile != null ? remotePluginFile.Plugin : null;
        }

        private IEnumerable<RemotePluginFile> GetPossibleRemotePluginFilesForFile(PluginFile file)
        {
            var fileInfo = new FileInfo(file.Path);
            return remotePluginFileController.Files.Where(
                    x => x.Name.Equals(fileInfo.Name, StringComparison.OrdinalIgnoreCase) && x.Checksum.Equals(file.Checksum));
        }
    }
}
