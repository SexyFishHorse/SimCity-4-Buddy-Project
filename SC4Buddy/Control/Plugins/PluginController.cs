﻿namespace NIHEI.SC4Buddy.Control.Plugins
{
    using System.Collections.Generic;
    using System.Linq;

    using NIHEI.SC4Buddy.DataAccess.Plugins;
    using NIHEI.SC4Buddy.Entities;

    public class PluginController
    {
        private readonly IPluginRegistry registry;

        public PluginController(IPluginRegistry registry)
        {
            this.registry = registry;
        }

        public IEnumerable<Plugin> Plugins
        {
            get
            {
                return registry.Plugins;
            }
        }

        public void Add(Plugin plugin)
        {
            registry.Add(plugin);
        }

        public void Update(Plugin plugin)
        {
            registry.Update(plugin);
        }

        public void Delete(Plugin plugin)
        {
            registry.Delete(plugin);
        }

        public void RemoveEmptyPlugins()
        {
            var emptyPlugins = registry.Plugins.Where(x => !x.Files.Any());

            foreach (var emptyPlugin in emptyPlugins)
            {
                Delete(emptyPlugin);
            }
        }
    }
}
