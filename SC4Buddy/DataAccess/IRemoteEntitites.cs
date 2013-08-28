namespace NIHEI.SC4Buddy.DataAccess
{
    using System.Linq;

    public interface IRemoteEntitites
    {
        IQueryable<Author> Authors { get; }

        IQueryable<RemotePlugin> Plugins { get; }

        IQueryable<RemotePluginFile> PluginFiles { get; }

        IQueryable<User> Users { get; }

        void SaveChanges();
    }
}
