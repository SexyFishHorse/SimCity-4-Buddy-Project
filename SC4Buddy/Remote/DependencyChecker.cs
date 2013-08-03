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
            var missingDependencies = GetMissingDependenciesForFolder(userFolder);

            var controller = new UserFolderController(RegistryFactory.UserFolderRegistry);

            if (!controller.IsMainFolder(userFolder))
            {
                var mainMissingDependencies = GetMissingDependenciesForFolder(controller.GetMainUserFolder());

                var extraMissing = mainMissingDependencies.Where(mainMissing => !missingDependencies.Any()).ToList();

                missingDependencies.AddRange(extraMissing);
            }

            return missingDependencies;
        }

        private static List<RemotePlugin> GetMissingDependenciesForFolder(UserFolder userFolder)
        {
            var knownPlugins =
                userFolder.Plugins.Where(x => x.RemotePluginId > 0 && x.UserFolder.Equals(userFolder)).ToList();

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

            var missingDependencies = new List<RemotePlugin>();

            foreach (var remotePlugin in remotePluginsWithDependencies)
            {
                foreach (var dependency in remotePlugin.Dependencies)
                {
                    if (!knownPlugins.Any(x => x.RemotePluginId == dependency.Id))
                    {
                        missingDependencies.Add(dependency);
                    }
                }
            }

            return missingDependencies;
        }
    }
}