namespace NIHEI.SC4Buddy.Control.Remote
{
    using System;
    using System.Data.Objects;
    using System.Linq;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities.Remote;

    public class RemotePluginController
    {
        private readonly IRemoteEntities entities;

        public RemotePluginController(IRemoteEntities entities)
        {
            this.entities = entities;
        }

        public IObjectSet<RemotePlugin> RemotePlugins
        {
            get
            {
                return entities.Plugins;
            }
        }

        public void Add(RemotePlugin remotePlugin)
        {
            entities.Plugins.AddObject(remotePlugin);
            SaveChanges();
        }

        public void SaveChanges()
        {
            entities.SaveChanges();
        }

        public void Delete(RemotePlugin item)
        {
            var files = item.PluginFiles.ToList();

            foreach (var file in files)
            {
                entities.PluginFiles.DeleteObject(file);
            }

            entities.Plugins.DeleteObject(item);
            entities.SaveChanges();
        }

        public static bool ValidateLinkAndAuthor(string link, Author author)
        {
            var siteUri = new UriBuilder(author.Site);
            var linkUri = new UriBuilder(link);

            return linkUri.Host.EndsWith(siteUri.Host, StringComparison.OrdinalIgnoreCase);
        }
    }
}
