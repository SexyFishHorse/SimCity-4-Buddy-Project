namespace NIHEI.SC4Buddy.Remote
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.DataAccess.Plugins;
    using NIHEI.SC4Buddy.DataAccess.Remote;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Entities.Remote;

    public class PluginMatcher
    {
        private readonly RemotePluginFileRegistry remotePluginFileRegistry;

        private readonly PluginRegistry pluginRegistry;

        public PluginMatcher()
        {
            remotePluginFileRegistry = RemoteRegistryFactory.RemotePluginFileRegistry;

            pluginRegistry = RegistryFactory.PluginRegistry;
        }

        public IEnumerable<RemotePlugin> GetPossibleRemotePlugins(IList<PluginFile> files)
        {
            var fileMatches = new List<RemotePluginFile>(files.Count());

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file.Path);
                fileMatches.AddRange(
                    remotePluginFileRegistry.Files.Where(
                        x =>
                        x.Name.Equals(fileInfo.Name, StringComparison.OrdinalIgnoreCase) && x.Checksum.Equals(file.Checksum)));
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
            if (remotePlugin != null)
            {
                plugin.RemotePluginId = remotePlugin.Id;
                plugin.Name = remotePlugin.Name;
                plugin.Author = remotePlugin.Author;
                plugin.Link = remotePlugin.Link;
                plugin.Description = remotePlugin.Description;

                pluginRegistry.Update(plugin);

                return true;
            }

            return false;
        }
    }
}
