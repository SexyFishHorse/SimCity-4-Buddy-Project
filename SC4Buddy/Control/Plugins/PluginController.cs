namespace NIHEI.SC4Buddy.Control.Plugins
{
    using System.Data.Objects;
    using System.Linq;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities;

    public class PluginController
    {
        private readonly IEntities entities;

        public PluginController(IEntities entities)
        {
            this.entities = entities;
        }

        public IObjectSet<Plugin> Plugins
        {
            get
            {
                return entities.Plugins;
            }
        }

        public void Add(Plugin plugin)
        {
            entities.Plugins.AddObject(plugin);
        }

        public void SaveChanges()
        {
            entities.SaveChanges();
        }

        public void Delete(Plugin plugin)
        {
            entities.Plugins.DeleteObject(plugin);
        }

        public int RemoveEmptyPlugins()
        {
            var emptyPlugins = entities.Plugins.Where(x => !x.Files.Any()).ToList();

            var counter = emptyPlugins.Count();

            foreach (var emptyPlugin in emptyPlugins)
            {
                Delete(emptyPlugin);
            }

            return counter;
        }
    }
}
