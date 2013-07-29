namespace NIHEI.SC4Buddy.DataAccess.Remote
{
    using System.Configuration;
    using System.Data.EntityClient;

    using NIHEI.SC4Buddy.Entities.Remote;

    public class RemoteRegistryFactory
    {
        private static readonly RemoteRegistryFactory Instance = new RemoteRegistryFactory();

        private readonly RemotePluginRegistry remotePluginRegistry;

        private readonly RemotePluginFileRegistry remotePluginFileRegistry;

        private readonly UserRegistry userRegistry;

        private readonly AuthorRegistry authorRegistry;

        private RemoteRegistryFactory()
        {
            var originalConnectionString = ConfigurationManager.ConnectionStrings["RemoteEntities"].ConnectionString;

            var entityBuilder = new EntityConnectionStringBuilder(originalConnectionString);
            var providerConnectionString = entityBuilder.ProviderConnectionString;
            providerConnectionString += ";password=_VslefXPl5Tg8pBcSYzI";

            entityBuilder.ProviderConnectionString = providerConnectionString;

            var entities = new RemoteEntities(entityBuilder.ConnectionString);

            remotePluginRegistry = new RemotePluginRegistry(entities);

            remotePluginFileRegistry = new RemotePluginFileRegistry(entities);

            userRegistry = new UserRegistry(entities);

            authorRegistry = new AuthorRegistry(entities);
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

        public static AuthorRegistry AuthorRegistry
        {
            get
            {
                return Instance.authorRegistry;
            }
        }
    }
}
