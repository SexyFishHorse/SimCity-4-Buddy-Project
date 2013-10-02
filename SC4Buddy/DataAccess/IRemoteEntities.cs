namespace NIHEI.SC4Buddy.DataAccess
{
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;

    using SC4Buddy.Entities.Remote;

    public interface IRemoteEntities
    {
        IObjectSet<Author> Authors { get; }

        IObjectSet<RemotePlugin> Plugins { get; }

        IObjectSet<RemotePluginFile> PluginFiles { get; }

        IObjectSet<User> Users { get; }

        void SaveChanges();

        void RevertChanges(EntityObject entityObject);

        void RevertChanges(IEnumerable<EntityObject> entityCollection);
    }
}
