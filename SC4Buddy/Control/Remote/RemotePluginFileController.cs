namespace NIHEI.SC4Buddy.Control.Remote
{
    using System.Collections.Generic;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities.Remote;

    public class RemotePluginFileController
    {
        private readonly IRemoteEntities entities;

        public RemotePluginFileController(IRemoteEntities entities)
        {
            this.entities = entities;
        }

        public ICollection<RemotePluginFile> Files
        {
            get
            {
                return entities.PluginFiles;
            }
        }

        public void Add(RemotePluginFile remoteFile)
        {
            Files.Add(remoteFile);
            SaveChanges();
        }

        public void SaveChanges()
        {
            entities.SaveChanges();
        }
    }
}
