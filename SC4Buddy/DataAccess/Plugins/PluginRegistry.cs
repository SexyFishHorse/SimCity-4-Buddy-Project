namespace NIHEI.SC4Buddy.DataAccess.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NIHEI.SC4Buddy.Entities;

    public interface IPluginRegistry
    {
        ICollection<Plugin> Plugins { get; }

        void Add(Plugin plugin);

        void Update(Plugin plugin);

        void Delete(Plugin plugin);
    }

    public class PluginRegistry : IPluginRegistry
    {
        private readonly DatabaseEntities entities;

        public PluginRegistry(DatabaseEntities databaseEntities)
        {
            entities = databaseEntities;
        }

        public ICollection<Plugin> Plugins
        {
            get
            {
                return entities.Plugins.ToList();
            }
        }

        public void Add(Plugin plugin)
        {
            entities.Plugins.AddObject(plugin);
            entities.SaveChanges();
        }

        public void Update(Plugin plugin)
        {
            if (plugin.Id < 1)
            {
                throw new ArgumentException(@"Plugin must have an id", "plugin");
            }

            if (entities.Plugins.Any(x => x.Id == plugin.Id))
            {
                entities.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException(
                    "Plugin not in collection (did you change the id or forgot to add it first?)");
            }
        }

        public void Delete(Plugin plugin)
        {
            entities.Plugins.DeleteObject(plugin);
            entities.SaveChanges();
        }
    }
}
