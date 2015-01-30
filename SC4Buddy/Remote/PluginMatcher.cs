namespace NIHEI.SC4Buddy.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Asser.Sc4Buddy.Server.Api.V1.Client;
    using Asser.Sc4Buddy.Server.Api.V1.Models;
    using MoreLinq;
    using NIHEI.SC4Buddy.Model;
    using Plugin = Asser.Sc4Buddy.Server.Api.V1.Models.Plugin;

    public class PluginMatcher : IPluginMatcher
    {
        private readonly IEnumerable<Plugin> plugins;

        private readonly IEnumerable<File> files;

        public PluginMatcher(IBuddyServerClient client)
        {
            plugins = client.GetAllPlugins();
            files = client.GetAllFiles();
        }

        public Plugin GetMostLikelyPluginForGroupOfFiles(IEnumerable<PluginFile> fileInfos)
        {
            var matchedPlugins = new Dictionary<Guid, int>();

            foreach (var fileInfo in fileInfos)
            {
                var fileInfoClosure = fileInfo;
                var matchedPlugin = Guid.Empty;
                foreach (var file in files.Where(x => x.Filename == fileInfoClosure.Filename && x.Checksum == fileInfoClosure.Checksum))
                {
                    matchedPlugin = file.Plugin;
                    break;
                }

                if (matchedPlugin == Guid.Empty)
                {
                    continue;
                }

                if (matchedPlugins.ContainsKey(matchedPlugin))
                {
                    matchedPlugins[matchedPlugin] = matchedPlugins[matchedPlugin]++;
                }
                else
                {
                    matchedPlugins.Add(matchedPlugin, 1);
                }
            }

            return matchedPlugins.Any() ? plugins.FirstOrDefault(x => x.Id == matchedPlugins.MaxBy(y => y.Value).Key) : null;
        }
    }
}
