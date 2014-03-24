namespace NIHEI.SC4Buddy.DataAccess
{
    using System;
    using System.Collections.Generic;

    using NIHEI.SC4Buddy.Model;

    public interface IEntities : IDisposable
    {
        ICollection<Plugin> Plugins { get; }

        ICollection<PluginFile> Files { get; }

        ICollection<UserFolder> UserFolders { get; }

        ICollection<PluginGroup> Groups { get; }

        void SaveChanges();

        void RevertChanges(ModelBase entityObject);

        void RevertChanges(IEnumerable<ModelBase> entityCollection);
    }
}
