namespace NIHEI.SC4Buddy.DataAccess
{
    using System.Linq;

    public class RemoteEntities : IRemoteEntitites
    {
        public IQueryable<Author> Authors { get; private set; }

        public IQueryable<RemotePlugin> Plugins { get; private set; }

        public IQueryable<RemotePluginFile> PluginFiles { get; private set; }

        public IQueryable<User> Users { get; private set; }

        public void SaveChanges()
        {
            throw new System.NotImplementedException();
        }
    }
}
