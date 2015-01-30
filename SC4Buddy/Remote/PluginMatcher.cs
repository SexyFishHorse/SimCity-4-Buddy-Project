namespace NIHEI.SC4Buddy.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Asser.Sc4Buddy.Server.Api.Client.V1.Models;
    using Asser.Sc4Buddy.Server.Api.V1.Client;
    using MoreLinq;
    using NIHEI.SC4Buddy.Model;
    using Plugin = Asser.Sc4Buddy.Server.Api.Client.V1.Models.Plugin;

    public class PluginMatcher : IPluginMatcher
    {
        private readonly IEnumerable<Plugin> plugins;

        private readonly IEnumerable<File> files;

        public PluginMatcher(IBuddyServerClient client)
        {
            plugins = client.GetAllPlugins();
            files = client.GetAllFiles();
        }

        public Plugin GetMostLikelyPluginForFiles(IEnumerable<PluginFile> fileInfos)
        {
            var matchedPlugins = new Dictionary<Guid, int>();

            foreach (var fileInfo in fileInfos)
            {
                var matchedPlugin = files.FirstOrDefault(x => x.Filename == fileInfo.Filename && x.Checksum == fileInfo.Checksum);

                if (matchedPlugin == null)
                {
                    continue;
                }

                if (matchedPlugins.ContainsKey(matchedPlugin.Id))
                {
                    matchedPlugins[matchedPlugin.Id] = matchedPlugins[matchedPlugin.Id]++;
                }
                else
                {
                    matchedPlugins.Add(matchedPlugin.Id, 1);
                }
            }

            return plugins.FirstOrDefault(x => x.Id == matchedPlugins.MaxBy(y => y.Value).Key);
        }
    }
}
