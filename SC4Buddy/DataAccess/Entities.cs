namespace NIHEI.SC4Buddy.DataAccess
{
    using System.Linq;

    using NIHEI.SC4Buddy.Entities;

    public class Entities : IEntities
    {
        public IQueryable<Plugin> Plugins { get; private set; }

        public IQueryable<PluginFile> Files { get; private set; }

        public IQueryable<UserFolder> UserFolders { get; private set; }

        public IQueryable<PluginGroup> Groups { get; private set; }

        public void SaveChanges()
        {
            throw new System.NotImplementedException();
        }
    }
}
