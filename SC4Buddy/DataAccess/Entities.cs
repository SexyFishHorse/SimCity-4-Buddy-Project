namespace NIHEI.SC4Buddy.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Objects;

    using NIHEI.SC4Buddy.Model;

    public class Entities : IEntities
    {
        private string StorageLocation { get; set; }

        public Entities(string storageLocation)
        {
            StorageLocation = storageLocation;
        }

        public IObjectSet<Plugin> Plugins { get; private set; }

        public IObjectSet<PluginFile> Files { get; private set; }

        public IObjectSet<UserFolder> UserFolders { get; set; }

        public IObjectSet<PluginGroup> Groups { get; set; }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void RevertChanges(ModelBase entityObject)
        {
        }

        public void RevertChanges(IEnumerable<ModelBase> entityCollection)
        {
        }

        public void Dispose()
        {
        }

        public void LoadAllEntitiesFromDisc()
        {
            throw new NotImplementedException();
        }
    }
}
