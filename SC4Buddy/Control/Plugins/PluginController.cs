﻿namespace NIHEI.SC4Buddy.Control.Plugins
{
    using System.Collections.Generic;
    using System.Linq;

    using DataAccess;

    using NIHEI.SC4Buddy.Model;

    public class PluginController
    {
        private readonly IEntities entities;

        public PluginController(IEntities entities)
        {
            this.entities = entities;
        }

        public ICollection<Plugin> Plugins
        {
            get
            {
                return entities.Plugins;
            }
        }

        public void Add(Plugin plugin, bool save = true)
        {
            Plugins.Add(plugin);
            if (save)
            {
                SaveChanges();
            }
        }

        public void SaveChanges()
        {
            entities.SaveChanges();
        }

        public void Delete(Plugin plugin, bool save = true)
        {
            Plugins.Remove(plugin);
            if (save)
            {
                SaveChanges();
            }
        }

        public int RemoveEmptyPlugins()
        {
            var emptyPlugins = entities.Plugins.Where(x => !x.PluginFiles.Any()).ToList();

            var counter = emptyPlugins.Count();

            foreach (var emptyPlugin in emptyPlugins)
            {
                Delete(emptyPlugin);
            }

            return counter;
        }

        public void RevertChanges(Plugin selectedPlugin)
        {
            entities.RevertChanges(selectedPlugin);
        }
    }
}
