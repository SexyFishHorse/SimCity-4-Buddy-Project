﻿namespace NIHEI.SC4Buddy.DataAccess
{
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;

    using NIHEI.SC4Buddy.Entities;

    public interface IEntities
    {
        IObjectSet<Plugin> Plugins { get; }

        IObjectSet<PluginFile> Files { get; }

        IObjectSet<UserFolder> UserFolders { get; }

        IObjectSet<PluginGroup> Groups { get; }

        IObjectSet<QuarantinedFile> QuarantinedFiles { get; } 

        void SaveChanges();

        void RevertChanges(EntityObject entityObject);

        void RevertChanges(IEnumerable<EntityObject> entityCollection);
    }
}
