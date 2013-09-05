using System.Collections.Generic;
using System.Data.Objects.DataClasses;

namespace NIHEI.SC4Buddy.DataAccess
{
    using System.Data.Objects;

    using SC4Buddy.Entities;

    public interface IEntities
    {
        IObjectSet<Plugin> Plugins { get; }

        IObjectSet<PluginFile> Files { get; }

        IObjectSet<UserFolder> UserFolders { get; }

        IObjectSet<PluginGroup> Groups { get; }

        void SaveChanges();

        void RevertChanges(EntityObject entityObject);

        void RevertChanges(ICollection<EntityObject> entityCollection);
    }
}
