namespace NIHEI.SC4Buddy.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Irradiated.Sc4Buddy.ApiClient;

    using NIHEI.SC4Buddy.Model;

    using RemotePlugin = Irradiated.Sc4Buddy.ApiClient.Model.Plugin;

    public class DependencyChecker : IDependencyChecker
    {
        private readonly ISc4BuddyApiClient client;

        private readonly UserFolder mainUserFolder;

        public DependencyChecker(ISc4BuddyApiClient client, UserFolder mainUserFolder)
        {
            this.client = client;
            this.mainUserFolder = mainUserFolder;
        }

        public async Task<IEnumerable<RemotePlugin>> CheckDependenciesAsync(UserFolder userFolder)
        {
            var knownPlugins = GetAllKnownPluginGuids(userFolder);

            var pluginsWithDependencies = userFolder.Plugins
                .Where(x => x.RemotePlugin != null)
                .Where(x => x.RemotePlugin.PluginDependencies != null)
                .Where(x => x.RemotePlugin.PluginDependencies.Any());

            var missingDependencyGuids = new HashSet<Guid>();

            foreach (var dependency in pluginsWithDependencies
                .SelectMany(x => x.RemotePlugin.PluginDependencies)
                .Where(dependency => !knownPlugins.Contains(dependency)))
            {
                missingDependencyGuids.Add(dependency);
            }

            var plugins = await client.GetPluginsAsync(missingDependencyGuids);

            return plugins.Plugins;
        }

        private HashSet<Guid> GetAllKnownPluginGuids(UserFolder userFolder)
        {
            var knownUserFolderPlugins = userFolder.Plugins.Where(x => x.RemotePlugin != null);
            var knownMainPlugins = mainUserFolder.Plugins.Where(x => x.RemotePlugin != null);

            var knownPlugins = new HashSet<Guid>();

            foreach (var plugin in knownMainPlugins)
            {
                knownPlugins.Add(plugin.RemotePlugin.Id);
            }

            foreach (var plugin in knownUserFolderPlugins)
            {
                knownPlugins.Add(plugin.RemotePlugin.Id);
            }

            return knownPlugins;
        }
    }
}
