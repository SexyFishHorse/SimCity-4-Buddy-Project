namespace NIHEI.SC4Buddy.Control.Plugins
{
    using System.Data.Objects;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Model;

    public class PluginGroupController
    {
        private readonly IEntities entities;

        public PluginGroupController(IEntities entities)
        {
            this.entities = entities;
        }

        public IObjectSet<PluginGroup> Groups
        {
            get
            {
                return entities.Groups;
            }
        }

        public void Delete(PluginGroup pluginGroup)
        {
            Groups.DeleteObject(pluginGroup);
            SaveChanges();
        }

        public void SaveChanges()
        {
            entities.SaveChanges();
        }

        public void Add(PluginGroup pluginGroup)
        {
            entities.Groups.AddObject(pluginGroup);
            SaveChanges();
        }
    }
}
