namespace NIHEI.SC4Buddy.DataAccess
{
    using System.Linq;

    public class RemoteEntities : IRemoteEntitites
    {
        private readonly RemoteDatabaseEntities entities;

        public RemoteEntities(RemoteDatabaseEntities entities)
        {
            this.entities = entities;
        }

        public IQueryable<Author> Authors
        {
            get
            {
                return entities.Authors;
            }
        }

        public IQueryable<RemotePlugin> Plugins
        {
            get
            {
                return entities.RemotePlugins;
            }
        }

        public IQueryable<RemotePluginFile> PluginFiles
        {
            get
            {
                return entities.RemotePluginFiles;
            }
        }

        public IQueryable<User> Users
        {
            get
            {
                return entities.Users;
            }
        }

        public void SaveChanges()
        {
            entities.SaveChanges();
        }
    }
}
