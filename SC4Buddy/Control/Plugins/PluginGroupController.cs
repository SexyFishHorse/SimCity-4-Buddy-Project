namespace NIHEI.SC4Buddy.Control.Plugins
{
    using System.Collections.Generic;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Model;

    public class PluginGroupController
    {
        private readonly IEntities entities;

        public PluginGroupController(IEntities entities)
        {
            this.entities = entities;
        }

        public ICollection<PluginGroup> Groups
        {
            get
            {
                return entities.Groups;
            }
        }

        public void Delete(PluginGroup pluginGroup)
        {
            Groups.Remove(pluginGroup);
            SaveChanges();
        }

        public void SaveChanges()
        {
            entities.SaveChanges();
        }

        public void Add(PluginGroup pluginGroup)
        {
            entities.Groups.Add(pluginGroup);
            SaveChanges();
        }
    }
}
