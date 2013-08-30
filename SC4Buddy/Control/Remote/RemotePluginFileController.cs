namespace NIHEI.SC4Buddy.Control.Remote
{
    using System.Data.Objects;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities.Remote;

    public class RemotePluginFileController
    {
        private readonly IRemoteEntities entities;

        public RemotePluginFileController(IRemoteEntities entities)
        {
            this.entities = entities;
        }

        public IObjectSet<RemotePluginFile> Files
        {
            get
            {
                return entities.PluginFiles;
            }
        }

        public void Add(RemotePluginFile remoteFile)
        {
            Files.AddObject(remoteFile);
            SaveChanges();
        }

        public void SaveChanges()
        {
            entities.SaveChanges();
        }
    }
}
