namespace NIHEI.SC4Buddy.Remote
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Security.Policy;
    using System.Threading.Tasks;

    using Irradiated.Sc4Buddy.ApiClient;
    using Irradiated.Sc4Buddy.ApiClient.Model;

    using Plugin = NIHEI.SC4Buddy.Model.Plugin;
    using RemotePlugin = Irradiated.Sc4Buddy.ApiClient.Model.Plugin;
    using RemotePluginFile = Irradiated.Sc4Buddy.ApiClient.Model.PluginFile;

    public class PluginMatcher : IPluginMatcher
    {
        private readonly ISc4BuddyApiClient client;

        public PluginMatcher(ISc4BuddyApiClient client)
        {
            this.client = client;
        }

        public async Task<bool> MatchAndUpdateAsync(Plugin plugin)
        {
            var filesAndChecksums =
                plugin.PluginFiles.Select(
                    x => new PluginFileMetaInfo { Filename = new FileInfo(x.Path).Name, Checksum = x.Checksum });
            var possiblePlugins = await GetMostLikelyRemotePluginsForFilesAsync(filesAndChecksums);

            if (!possiblePlugins.Any())
            {
                return false;
            }

            var allPossiblePlugins = possiblePlugins.SelectMany(x => x.Value);
            var groupedPlugins = allPossiblePlugins.GroupBy(x => x.Id).ToList();
            var mostLikelyPlugin = groupedPlugins.First(x => x.Count() == groupedPlugins.Max(y => y.Count())).First();

            plugin.RemotePlugin = mostLikelyPlugin;
            plugin.Name = mostLikelyPlugin.Name;
            plugin.Description = mostLikelyPlugin.Description;
            plugin.Author = mostLikelyPlugin.AuthorName;
            plugin.Link = new Url(mostLikelyPlugin.LinkToDownloadPage);

            return true;
        }

        public async Task<IDictionary<PluginFileMetaInfo, IEnumerable<RemotePlugin>>> GetMostLikelyRemotePluginsForFilesAsync(IEnumerable<PluginFileMetaInfo> filePathAndChecksum)
        {
            return await GetMostLikelyPluginsForFilesAsync(filePathAndChecksum);
        }

        public async Task<IEnumerable<RemotePlugin>> GetMostLikelyRemotePluginForFileAsync(string filepath, string checksum)
        {
            var filename = new FileInfo(filepath).Name;
            var pluginFileMetaInfos = new Collection<PluginFileMetaInfo>
                                      {
                                          new PluginFileMetaInfo
                                          {
                                              Filename =
                                                  filename,
                                              Checksum =
                                                  checksum
                                          }
                                      };

            var guidCollection = await client.GetPluginsByFileInfoAsync(pluginFileMetaInfos);
            var guid = guidCollection.SelectMany(x => x.Value).ToList();

            if (!guid.Any())
            {
                return new Collection<RemotePlugin>();
            }

            var plugins = await client.GetPluginsAsync(guid);

            return plugins.Plugins;
        }

        private async Task<IDictionary<PluginFileMetaInfo, IEnumerable<RemotePlugin>>> GetMostLikelyPluginsForFilesAsync(
            IEnumerable<PluginFileMetaInfo> pluginFileMetaInfos)
        {
            var pluginGuids = await client.GetPluginsByFileInfoAsync(pluginFileMetaInfos.ToList());
            var allGuids = pluginGuids.SelectMany(x => x.Value).GroupBy(x => x).Select(x => x.Key);
            var plugins = await client.GetPluginsAsync(allGuids);

            var dictionary = new Dictionary<PluginFileMetaInfo, IEnumerable<RemotePlugin>>();

            foreach (var entry in pluginGuids)
            {
                var entryPlugins = new Collection<RemotePlugin>();

                foreach (var plugin in entry.Value.Select(guid => plugins.Plugins.FirstOrDefault(x => x.Id == guid)))
                {
                    entryPlugins.Add(plugin);
                }

                dictionary.Add(entry.Key, entryPlugins);
            }

            return dictionary;
        }
    }
}
