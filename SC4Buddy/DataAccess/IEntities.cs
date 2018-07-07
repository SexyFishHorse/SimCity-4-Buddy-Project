namespace Nihei.SC4Buddy.DataAccess
{
    using System;
    using System.Collections.Generic;
    using Nihei.SC4Buddy.Model;

    public interface IEntities : IDisposable
    {
        ICollection<PluginGroup> Groups { get; }

        void SaveChanges();

        void RevertChanges(ModelBase entityObject);

        void RevertChanges(IEnumerable<ModelBase> entityCollection);
    }
}
