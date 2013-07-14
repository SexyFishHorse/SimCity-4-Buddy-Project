namespace NIHEI.SC4Buddy.DataAccess
{
    using NIHEI.SC4Buddy.DataAccess.Plugins;
    using NIHEI.SC4Buddy.Entities;

    public class RegistryFactory
    {
        private static readonly RegistryFactory Instance = new RegistryFactory();

        private readonly PluginFileRegistry pluginFileRegistry;

        private readonly UserFolderRegistry userFolderRegistry;

        private readonly PluginGroupRegistry pluginGroupRegistry;

        private readonly PluginRegistry pluginRegistry;

        private RegistryFactory()
        {
            var databaseEntities = new DatabaseEntities();
            pluginFileRegistry = new PluginFileRegistry(databaseEntities);
            userFolderRegistry = new UserFolderRegistry(databaseEntities);
            pluginGroupRegistry = new PluginGroupRegistry(databaseEntities);
            pluginRegistry = new PluginRegistry(databaseEntities);
        }

        public static PluginFileRegistry PluginFileRegistry
        {
            get
            {
                return Instance.pluginFileRegistry;
            }
        }

        public static UserFolderRegistry UserFolderRegistry
        {
            get
            {
                return Instance.userFolderRegistry;
            }
        }

        public static PluginGroupRegistry PluginGroupRegistry
        {
            get
            {
                return Instance.pluginGroupRegistry;
            }
        }

        public static PluginRegistry PluginRegistry
        {
            get
            {
                return Instance.pluginRegistry;
            }
        }
    }
}
