namespace NIHEI.SC4Buddy.DataAccess.Remote
{
    using System.Configuration;
    using System.Data.EntityClient;

    using NIHEI.SC4Buddy.Entities.Remote;

    public class RemoteRegistryFactory
    {
        private static readonly RemoteRegistryFactory Instance = new RemoteRegistryFactory();

        private readonly UserRegistry userRegistry;

        private RemoteRegistryFactory()
        {
            var originalConnectionString = ConfigurationManager.ConnectionStrings["RemoteEntities"].ConnectionString;

            var entityBuilder = new EntityConnectionStringBuilder(originalConnectionString);
            var providerConnectionString = entityBuilder.ProviderConnectionString;
            providerConnectionString += ";password=_VslefXPl5Tg8pBcSYzI";

            entityBuilder.ProviderConnectionString = providerConnectionString;

            var entities = new RemoteDatabaseEntities(entityBuilder.ConnectionString);

            userRegistry = new UserRegistry(entities);
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
