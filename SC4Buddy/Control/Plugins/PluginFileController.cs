namespace NIHEI.SC4Buddy.Control.Plugins
{
    using NIHEI.SC4Buddy.DataAccess.Plugins;
    using NIHEI.SC4Buddy.Entities;

    public class PluginFileController
    {
        private readonly IPluginFileRegistry registry;

        public PluginFileController(IPluginFileRegistry registry)
        {
            this.registry = registry;
        }

        public void Delete(PluginFile file)
        {
            registry.Delete(file);
        }
    }
}