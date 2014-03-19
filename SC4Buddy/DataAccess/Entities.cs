namespace NIHEI.SC4Buddy.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;

    using NIHEI.SC4Buddy.Model;

    public class Entities : IEntities
    {
        public IObjectSet<Plugin> Plugins
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IObjectSet<PluginFile> Files
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IObjectSet<UserFolder> UserFolders
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IObjectSet<PluginGroup> Groups
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void RevertChanges(EntityObject entityObject)
        {
        }

        public void RevertChanges(IEnumerable<EntityObject> entityCollection)
        {
        }

        public void Dispose()
        {
        }
    }
}
