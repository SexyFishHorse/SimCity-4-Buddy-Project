namespace NIHEI.SC4Buddy.DataAccess.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NIHEI.SC4Buddy.Entities;

    public interface IPluginGroupRegistry
    {
        ICollection<PluginGroup> PluginGroups { get; }

        void Add(PluginGroup pluginGroup);

        void Update(PluginGroup pluginGroup);

        void Delete(PluginGroup pluginGroup);
    }

    public class PluginGroupRegistry : IPluginGroupRegistry
    {
        private readonly DatabaseEntities entities;

        public PluginGroupRegistry(DatabaseEntities databaseEntities)
        {
            entities = databaseEntities;
        }

        public ICollection<PluginGroup> PluginGroups
        {
            get
            {
                return entities.PluginGroups.ToList();
            }
        }

        public void Add(PluginGroup pluginGroup)
        {
            entities.PluginGroups.AddObject(pluginGroup);
            entities.SaveChanges();
        }

        public void Update(PluginGroup pluginGroup)
        {
            if (pluginGroup.Id < 1)
            {
                throw new ArgumentException(@"Plugin group must have an id", "pluginGroup");
            }

            if (entities.PluginGroups.Any(x => x.Id == pluginGroup.Id))
            {
                entities.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException(
                    "Plugin group not in collection (did you change the id or forgot to add it first?)");
            }
        }

        public void Delete(PluginGroup pluginGroup)
        {
            entities.PluginGroups.DeleteObject(pluginGroup);
            entities.SaveChanges();
        }
    }
}
