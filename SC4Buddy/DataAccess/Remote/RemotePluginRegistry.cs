namespace NIHEI.SC4Buddy.DataAccess.Remote
{
    using System.Data.Objects;
    using System.Linq;

    using NIHEI.SC4Buddy.Entities.Remote;

    public class RemotePluginRegistry
    {
        private readonly RemoteEntities entities;

        public RemotePluginRegistry(RemoteEntities entities)
        {
            this.entities = entities;
        }

        public ObjectSet<RemotePlugin> RemotePlugins
        {
            get
            {
                return entities.RemotePlugins;
            }
        }

        public void Add(RemotePlugin remotePlugin)
        {
            entities.RemotePlugins.AddObject(remotePlugin);
            entities.SaveChanges();
        }

        public void Update(RemotePlugin dependency)
        {
            entities.SaveChanges();
        }

        public void Delete(RemotePlugin item)
        {
            var files = item.Files.ToList();

            foreach (var file in files)
            {
                entities.RemotePluginFiles.DeleteObject(file);
            }

            entities.RemotePlugins.DeleteObject(item);
            entities.SaveChanges();
        }
    }
}
