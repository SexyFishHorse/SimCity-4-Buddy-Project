namespace NIHEI.SC4Buddy.Remote
{
    using System.Collections.Generic;
    using System.Linq;

    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.DataAccess.Remote;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Entities.Remote;

    public class DependencyChecker
    {
        public List<RemotePlugin> CheckDependencies(UserFolder userFolder)
        {
            var knownPluginsWithDependencies = GetPluginsWithDependencies(userFolder);
            var knownPlugins = GetKnownPlugins(userFolder);

            var missingDependencies = GetMissingDependencies(knownPluginsWithDependencies, knownPlugins);

            var controller = new UserFolderController(EntityFactory.Instance.Entities);

            if (!controller.IsMainFolder(userFolder))
            {
                var knownMainPlugins = GetKnownPlugins(controller.GetMainUserFolder());

                var mainMissingDependencies = GetMissingDependencies(
                    knownPluginsWithDependencies, knownMainPlugins);

                var commonMissing = new List<RemotePlugin>();
                foreach (var missingDependency in missingDependencies)
                {
                    foreach (var mainMissingDependency in mainMissingDependencies)
                    {
                        if (missingDependency.Id == mainMissingDependency.Id)
                        {
                            commonMissing.Add(missingDependency);
                        }
                    }
                }

                missingDependencies = commonMissing;
            }

            return missingDependencies;
        }

        private static List<RemotePlugin> GetPluginsWithDependencies(UserFolder userFolder)
        {
            var knownPlugins =
                userFolder.Plugins.Where(x => x.RemotePluginId > 0 && x.UserFolder.Id == userFolder.Id).ToList();

            var remotePluginRegistry = RemoteRegistryFactory.RemotePluginRegistry;

            var remotePluginsWithDependencies = new List<RemotePlugin>();

            foreach (var knownPlugin in knownPlugins)
            {
                foreach (var remotePlugin in remotePluginRegistry.RemotePlugins.Include("Author"))
                {
                    if (remotePlugin.Id == knownPlugin.RemotePluginId)
                    {
                        if (remotePlugin.Dependencies.Any())
                        {
                            remotePluginsWithDependencies.Add(remotePlugin);
                        }
                    }
                }
            }

            return remotePluginsWithDependencies;
        }

        private static List<RemotePlugin> GetKnownPlugins(UserFolder userFolder)
        {
            var knownPlugins =
                userFolder.Plugins.Where(x => x.RemotePluginId > 0 && x.UserFolder.Id == userFolder.Id).ToList();

            var remotePluginRegistry = RemoteRegistryFactory.RemotePluginRegistry;

            var remotePluginsWithDependencies = new List<RemotePlugin>();

            foreach (var knownPlugin in knownPlugins)
            {
                foreach (var remotePlugin in remotePluginRegistry.RemotePlugins.Include("Author"))
                {
                    if (remotePlugin.Id == knownPlugin.RemotePluginId)
                    {
                        remotePluginsWithDependencies.Add(remotePlugin);
                    }
                }
            }

            return remotePluginsWithDependencies;
        }

        private static List<RemotePlugin> GetMissingDependencies(List<RemotePlugin> installedPlugins, List<RemotePlugin> pluginsToCheckAgainst)
        {
            var allDependencies = installedPlugins.SelectMany(x => x.Dependencies).ToList();

            var installedDependency = new List<RemotePlugin>();
            foreach (RemotePlugin dependency in allDependencies)
            {
                foreach (RemotePlugin plugin in pluginsToCheckAgainst)
                {
                    if (dependency.Equals(plugin))
                    {
                        installedDependency.Add(dependency);
                    }
                }
            }

            foreach (var remotePlugin in installedDependency)
            {
                allDependencies.Remove(remotePlugin);
            }

            return allDependencies;
        }
    }
}