namespace NIHEI.SC4Buddy.Control.Plugins
{
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities;

    public class PluginFileController
    {
        private readonly IEntities entities;

        public PluginFileController(IEntities entities)
        {
            this.entities = entities;
        }

        public void Delete(PluginFile file)
        {
            entities.Files.DeleteObject(file);
        }
    }
}