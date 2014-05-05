namespace NIHEI.SC4Buddy.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Security.Policy;

    using Irradiated.Sc4Buddy.ApiClient;

    using NIHEI.SC4Buddy.Model;

    using RemotePlugin = Irradiated.Sc4Buddy.ApiClient.Model.Plugin;
    using RemotePluginFile = Irradiated.Sc4Buddy.ApiClient.Model.PluginFile;

    public class PluginMatcher : IPluginMatcher
    {
        private ISc4BuddyApiClient client;

        public PluginMatcher(ISc4BuddyApiClient client)
        {
            this.client = client;
        }

        public bool MatchAndUpdate(Plugin plugin)
        {
            var possiblePlugins = new Collection<RemotePlugin>();
            foreach (var pluginFile in plugin.PluginFiles)
            {
                possiblePlugins.Add(GetMostLikelyRemotePluginForFile(pluginFile.Path, pluginFile.Checksum));
            }

            if (!possiblePlugins.Any())
            {
                return false;
            }

            var groupedPlugins = possiblePlugins.GroupBy(x => x.Id).ToList();
            var mostLikelyPlugin = groupedPlugins.First(x => x.Count() == groupedPlugins.Max(y => y.Count())).First();

            plugin.RemotePlugin = mostLikelyPlugin;
            plugin.Name = mostLikelyPlugin.Name;
            plugin.Description = mostLikelyPlugin.Description;
            plugin.Author = mostLikelyPlugin.AuthorName;
            plugin.Link = new Url(mostLikelyPlugin.LinkToDownloadPage);

            return true;
        }

        public RemotePlugin GetMostLikelyRemotePluginForFile(string filePath, string checksum)
        {
            throw new NotImplementedException();
        }
    }
}
