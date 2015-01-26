namespace NIHEI.SC4Buddy.DataAccess
{
    using System;
    using System.Collections.Generic;

    using NIHEI.SC4Buddy.Model;

    public interface IEntities : IDisposable
    {
        ICollection<PluginFile> Files { get; }

        ICollection<PluginGroup> Groups { get; }

        void SaveChanges();

        void RevertChanges(ModelBase entityObject);

        void RevertChanges(IEnumerable<ModelBase> entityCollection);
    }
}
