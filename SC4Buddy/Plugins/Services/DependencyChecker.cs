﻿namespace NIHEI.SC4Buddy.Plugins.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Asser.Sc4Buddy.Server.Api.V1.Client;
    using log4net;
    using NIHEI.SC4Buddy.Model;

    public class DependencyChecker : IDependencyChecker
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly BuddyServerClient buddyServerClient;

        private readonly UserFolder mainUserFolder;

        public DependencyChecker(BuddyServerClient buddyServerClient, UserFolder mainUserFolder)
        {
            this.buddyServerClient = buddyServerClient;
            this.mainUserFolder = mainUserFolder;
        }

        public IEnumerable<Asser.Sc4Buddy.Server.Api.V1.Models.Plugin> CheckDependencies(UserFolder userFolder)
        {
            Log.Debug("Fetching all plugins from the server.");
            var allServerPlugins = buddyServerClient.GetAllPlugins().ToList();

            // Get all dependencies
            Log.Debug("Loading all dependencies.");
            var dependencies = new HashSet<Guid>();
            foreach (var dependency in userFolder.Plugins
                .Where(x => x.RemotePlugin != null)
                .Select(plugin => allServerPlugins.FirstOrDefault(x => x.Id == plugin.RemotePluginId))
                .Where(remotePlugin => remotePlugin != null)
                .SelectMany(remotePlugin => remotePlugin.Dependencies))
            {
                dependencies.Add(dependency);
            }

            Log.Debug(string.Format("{0} dependencies found in total.", dependencies.Count));

            Log.Debug(string.Format("Looping through {0} plugins in the main user folder.", mainUserFolder.Plugins.Count(x => x.RemotePlugin != null)));
            // Remove installed dependencies from the main user folder
            foreach (var plugin in mainUserFolder.Plugins.Where(x => x.RemotePlugin != null))
            {
                dependencies.Remove(plugin.RemotePluginId);
                Log.Debug(string.Format("Removed {0} ({1}) as a dependency", plugin.Name, plugin.RemotePluginId));
            }

            Log.Debug(string.Format("{0} dependencies remaining. Looping through {1} plugins in the user folder.", dependencies.Count, userFolder.Plugins.Count(x => x.RemotePlugin != null)));
            // Remove installed dependencies in the user folder
            foreach (var plugin in userFolder.Plugins.Where(x => x.RemotePlugin != null))
            {
                dependencies.Remove(plugin.RemotePluginId);
                Log.Debug(string.Format("Removed {0} ({1}) as a dependency", plugin.Name, plugin.RemotePluginId));
            }

            Log.Debug(string.Format("{0} dependencies remaining. Loading plugin data for said plugins.", dependencies.Count));
            return dependencies.Select(dependency => allServerPlugins.First(plugin => plugin.Id == dependency));
        }
    }
}