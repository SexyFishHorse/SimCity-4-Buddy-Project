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
            Plugins.AddObject(plugin);
            SaveChanges();
        }

        public void SaveChanges()
        {
            entities.SaveChanges();
        }

        public void Delete(Plugin plugin)
        {
            Plugins.DeleteObject(plugin);
            SaveChanges();
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
