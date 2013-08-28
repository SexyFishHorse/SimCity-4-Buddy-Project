namespace NIHEI.SC4Buddy.DataAccess
{
    using System.Linq;

    using NIHEI.SC4Buddy.Entities;

    public class Entities : IEntities
    {
        private readonly DatabaseEntities entities;

        public Entities(DatabaseEntities entities)
        {
            this.entities = entities;
        }

        public IQueryable<Plugin> Plugins
        {
            get
            {
                return entities.Plugins;
            }
        }

        public IQueryable<PluginFile> Files
        {
            get
            {
                return entities.PluginFiles;
            }
        }

        public IQueryable<UserFolder> UserFolders
        {
            get
            {
                return entities.UserFolders;
            }
        }

        public IQueryable<PluginGroup> Groups
        {
            get
            {
                return entities.PluginGroups;
            }
        }

        public void SaveChanges()
        {
            entities.SaveChanges();
        }
    }
}
