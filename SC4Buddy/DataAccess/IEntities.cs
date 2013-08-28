namespace NIHEI.SC4Buddy.DataAccess
{
    using System.Linq;

    using NIHEI.SC4Buddy.Entities;

    public interface IEntities
    {
        IQueryable<Plugin> Plugins { get; }

        IQueryable<PluginFile> Files { get; }

        IQueryable<UserFolder> UserFolders { get; }

        IQueryable<PluginGroup> Groups { get; }

        void SaveChanges();
    }
}
