namespace NIHEI.SC4Buddy.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Security.Policy;
    using System.Threading.Tasks;

    using Irradiated.Sc4Buddy.ApiClient;

    using NIHEI.SC4Buddy.Model;

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
            var filesAndChecksums = plugin.PluginFiles.Select(x => new Tuple<string, string>(x.Path, x.Checksum));
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

        public async Task<IDictionary<string, IEnumerable<RemotePlugin>>> GetMostLikelyRemotePluginsForFilesAsync(IEnumerable<Tuple<string, string>> filePathAndChecksum)
        {
            return await GetMostLikelyPluginsForFilesAsync(filePathAndChecksum);
        }

        public async Task<IEnumerable<RemotePlugin>> GetMostLikelyRemotePluginForFileAsync(string filepath, string checksum)
        {
            var filename = new FileInfo(filepath).Name;
            var filenameAndChecksum = new Collection<Tuple<string, string>>
                                          {
                                              new Tuple<string, string>(
                                                  filename,
                                                  checksum)
                                          };

            var guidCollection = await client.GetPluginsByFileInfoAsync(filenameAndChecksum);
            var guid = guidCollection.Select(x => x.Value).First();
            var plugins = await client.GetPluginsAsync(guid);

            return plugins.Plugins;
        }

        private async Task<IDictionary<string, IEnumerable<RemotePlugin>>> GetMostLikelyPluginsForFilesAsync(
            IEnumerable<Tuple<string, string>> filePathAndChecksum)
        {
            var fixedInput = new Collection<Tuple<string, string>>();
            foreach (var tuple in filePathAndChecksum)
            {
                fixedInput.Add(new Tuple<string, string>(new FileInfo(tuple.Item1).Name, tuple.Item2));
            }

            var pluginGuids = await client.GetPluginsByFileInfoAsync(fixedInput);
            var allGuids = pluginGuids.SelectMany(x => x.Value).GroupBy(x => x).Select(x => x.Key);
            var plugins = await client.GetPluginsAsync(allGuids);

            var dictionary = new Dictionary<string, IEnumerable<RemotePlugin>>();

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
