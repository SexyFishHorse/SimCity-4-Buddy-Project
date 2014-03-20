namespace NIHEI.SC4Buddy.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;

    using NIHEI.SC4Buddy.Model;

    public interface IEntities : IDisposable
    {
        IObjectSet<Plugin> Plugins { get; }

        IObjectSet<PluginFile> Files { get; }

        IObjectSet<UserFolder> UserFolders { get; }

        IObjectSet<PluginGroup> Groups { get; }

        void SaveChanges();

        void RevertChanges(ModelBase entityObject);

        void RevertChanges(IEnumerable<ModelBase> entityCollection);
    }
}
