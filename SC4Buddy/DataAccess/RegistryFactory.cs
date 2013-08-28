namespace NIHEI.SC4Buddy.DataAccess
{
    using NIHEI.SC4Buddy.DataAccess.Plugins;
    using NIHEI.SC4Buddy.Entities;

    public class RegistryFactory
    {
        private static readonly RegistryFactory Instance = new RegistryFactory();

        private readonly PluginGroupRegistry pluginGroupRegistry;

        private RegistryFactory()
        {
            var databaseEntities = new DatabaseEntities();
            pluginGroupRegistry = new PluginGroupRegistry(databaseEntities);
        }

        public static PluginGroupRegistry PluginGroupRegistry
        {
            get
            {
                return Instance.pluginGroupRegistry;
            }
        }
    }
}
