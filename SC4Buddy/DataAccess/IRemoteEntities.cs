namespace NIHEI.SC4Buddy.DataAccess
{
    using System;
    using System.Collections.Generic;

    using SC4Buddy.Entities.Remote;

    public interface IRemoteEntities : IDisposable
    {
        ICollection<Author> Authors { get; }

        ICollection<RemotePlugin> Plugins { get; }

        ICollection<RemotePluginFile> PluginFiles { get; }

        ICollection<User> Users { get; }

        void SaveChanges();

        void RevertChanges(object entityObject);

        void RevertChanges(IEnumerable<object> entityCollection);
    }
}
