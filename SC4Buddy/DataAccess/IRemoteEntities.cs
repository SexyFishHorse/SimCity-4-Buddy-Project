namespace NIHEI.SC4Buddy.DataAccess
{
    using System.Data.Objects;

    using SC4Buddy.Entities.Remote;

    public interface IRemoteEntities
    {
        IObjectSet<Author> Authors { get; }

        IObjectSet<RemotePlugin> Plugins { get; }

        IObjectSet<RemotePluginFile> PluginFiles { get; }

        IObjectSet<User> Users { get; }

        void SaveChanges();
    }
}
