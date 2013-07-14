namespace NIHEI.SC4Buddy.DataAccess.Remote
{
    using System.Collections.Generic;

    using NIHEI.SC4Buddy.Entities.Remote;

    public class RemotePluginFileRegistry
    {
        private readonly RemoteEntities entities;

        public RemotePluginFileRegistry(RemoteEntities entities)
        {
            this.entities = entities;
        }

        public IEnumerable<RemotePluginFile> Files
        {
            get
            {
                return entities.RemotePluginFiles;
            }
        }

        public void Add(RemotePluginFile remoteFile)
        {
            entities.RemotePluginFiles.AddObject(remoteFile);
            entities.SaveChanges();
        }
    }
}
