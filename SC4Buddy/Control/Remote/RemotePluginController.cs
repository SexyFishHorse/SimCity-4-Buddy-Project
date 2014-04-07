namespace NIHEI.SC4Buddy.Control.Remote
{
    using System;
    using System.Collections.Generic;
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

        public IEnumerable<RemotePlugin> Plugins
        {
            get
            {
                return entities.Plugins;
            }
        }

        public void Add(RemotePlugin remotePlugin)
        {
            entities.Plugins.Add(remotePlugin);
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
                entities.PluginFiles.Remove(file);
            }

            entities.Plugins.Remove(item);
        }

        public void RevertChanges(RemotePlugin remotePlugin)
        {
            entities.RevertChanges(remotePlugin);
        }

        public static bool ValidateLinkAndAuthor(string link, Author author)
        {
            var siteUri = new UriBuilder(author.Site);
            var linkUri = new UriBuilder(link);

            return linkUri.Host.EndsWith(siteUri.Host, StringComparison.OrdinalIgnoreCase);
        }

        public IEnumerable<RemotePlugin> SearchForPlugin(string text)
        {
            var upperText = text.ToUpper();
            return
                Plugins.Where(
                    x =>
                    x.Name.ToUpper().Contains(upperText)
                    || x.Link.ToUpper().Contains(upperText)
                    || x.Author.Name.ToUpper().Contains(upperText));
        }
    }
}
