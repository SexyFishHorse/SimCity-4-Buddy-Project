namespace NIHEI.SC4Buddy.Remote
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    using NIHEI.SC4Buddy.Control.Remote;
    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.Entities.Remote;
    using NIHEI.SC4Buddy.Model;

    public class DependencyChecker
    {
        private readonly UserFolderController userFolderController;

        private readonly RemotePluginController remotePluginController;

        public DependencyChecker(
            UserFolderController userFolderController, RemotePluginController remotePluginController)
        {
            this.userFolderController = userFolderController;
            this.remotePluginController = remotePluginController;
        }

        public List<RemotePlugin> CheckDependencies(UserFolder userFolder)
        {
            var knownPluginsWithDependencies = GetPluginsWithDependencies(userFolder);
            var knownPlugins = GetKnownPlugins(userFolder);

            var missingDependencies = GetMissingDependencies(knownPluginsWithDependencies, knownPlugins);

            if (!userFolderController.IsMainFolder(userFolder))
            {
                var knownMainPlugins = GetKnownPlugins(userFolderController.GetMainUserFolder());

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

        private List<RemotePlugin> GetPluginsWithDependencies(UserFolder userFolder)
        {
            var knownPlugins =
                userFolder.Plugins.Where(x => x.RemotePluginId > 0 && x.UserFolder.Id == userFolder.Id).ToList();

            var remotePluginsWithDependencies = new List<RemotePlugin>();

            foreach (var knownPlugin in knownPlugins)
            {
                foreach (var remotePlugin in remotePluginController.Plugins.Include("Author"))
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

        private List<RemotePlugin> GetKnownPlugins(UserFolder userFolder)
        {
            var knownPlugins =
                userFolder.Plugins.Where(x => x.RemotePluginId > 0 && x.UserFolder.Id == userFolder.Id).ToList();

            var remotePluginsWithDependencies = new List<RemotePlugin>();

            foreach (var knownPlugin in knownPlugins)
            {
                foreach (var remotePlugin in remotePluginController.Plugins.Include("Author"))
                {
                    if (remotePlugin.Id == knownPlugin.RemotePluginId)
                    {
                        remotePluginsWithDependencies.Add(remotePlugin);
                    }
                }
            }

            return remotePluginsWithDependencies;
        }

        private static List<RemotePlugin> GetMissingDependencies(
            List<RemotePlugin> installedPlugins, List<RemotePlugin> pluginsToCheckAgainst)
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