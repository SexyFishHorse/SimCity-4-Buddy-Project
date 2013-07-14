namespace NIHEI.SC4Buddy.DataAccess.Remote
{
    using NIHEI.SC4Buddy.Entities.Remote;

    public class RemoteRegistryFactory
    {
        private static readonly RemoteRegistryFactory Instance = new RemoteRegistryFactory();

        private readonly RemotePluginRegistry remotePluginRegistry;

        private readonly RemotePluginFileRegistry remotePluginFileRegistry;

        private readonly UserRegistry userRegistry;

        private RemoteRegistryFactory()
        {
            var entities = new RemoteEntities();

            remotePluginRegistry = new RemotePluginRegistry(entities);

            remotePluginFileRegistry = new RemotePluginFileRegistry(entities);

            userRegistry = new UserRegistry(entities);
        }

        public static RemotePluginRegistry RemotePluginRegistry
        {
            get
            {
                return Instance.remotePluginRegistry;
            }
        }

        public static RemotePluginFileRegistry RemotePluginFileRegistry
        {
            get
            {
                return Instance.remotePluginFileRegistry;
            }
        }

        public static UserRegistry UserRegistry
        {
            get
            {
                return Instance.userRegistry;
            }
        }
    }
}
