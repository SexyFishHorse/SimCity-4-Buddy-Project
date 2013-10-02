namespace NIHEI.SC4Buddy.DataAccess
{
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;

    using SC4Buddy.Entities.Remote;

    public class RemoteEntities : IRemoteEntities
    {
        private readonly RemoteDatabaseEntities entities;

        public RemoteEntities(RemoteDatabaseEntities entities)
        {
            this.entities = entities;
        }

        public IObjectSet<Author> Authors
        {
            get
            {
                return entities.Authors;
            }
        }

        public IObjectSet<RemotePlugin> Plugins
        {
            get
            {
                return entities.RemotePlugins;
            }
        }

        public IObjectSet<RemotePluginFile> PluginFiles
        {
            get
            {
                return entities.RemotePluginFiles;
            }
        }

        public IObjectSet<User> Users
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

        public void RevertChanges(EntityObject entityObject)
        {
            entities.Refresh(RefreshMode.StoreWins, entityObject);
        }

        public void RevertChanges(IEnumerable<EntityObject> entityCollection)
        {
            entities.Refresh(RefreshMode.StoreWins, entityCollection);
        }
    }
}
