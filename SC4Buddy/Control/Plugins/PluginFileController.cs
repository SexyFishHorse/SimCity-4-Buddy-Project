namespace NIHEI.SC4Buddy.Control.Plugins
{
    using System.Data.Objects;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities;

    public class PluginFileController
    {
        private readonly IEntities entities;

        public PluginFileController(IEntities entities)
        {
            this.entities = entities;
        }

        public IObjectSet<PluginFile> Files
        {
            get
            {
                return entities.Files;
            }
        }

        public void Delete(PluginFile file)
        {
            entities.Files.DeleteObject(file);
        }

        public void SaveChanges()
        {
            entities.SaveChanges();
        }
    }
}