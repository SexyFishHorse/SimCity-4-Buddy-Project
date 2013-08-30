namespace NIHEI.SC4Buddy.DataAccess
{
    using System.Configuration;
    using System.Data.EntityClient;

    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Entities.Remote;

    public class EntityFactory
    {
        private static EntityFactory instance;

        public static EntityFactory Instance
        {
            get
            {
                return instance ?? (instance = new EntityFactory());
            }
        }

        private EntityFactory()
        {
            RemoteEntities = CreateRemoteEntities();

            Entities = new Entities(new DatabaseEntities());
        }

        public Entities Entities { get; private set; }

        public RemoteEntities RemoteEntities { get; private set; }

        private RemoteEntities CreateRemoteEntities()
        {
            var originalConnectionString = ConfigurationManager.ConnectionStrings["RemoteDatabaseEntities"].ConnectionString;

            var entityBuilder = new EntityConnectionStringBuilder(originalConnectionString);
            var providerConnectionString = entityBuilder.ProviderConnectionString;
            providerConnectionString += ";password=_VslefXPl5Tg8pBcSYzI";

            entityBuilder.ProviderConnectionString = providerConnectionString;

            var entities = new RemoteDatabaseEntities(entityBuilder.ConnectionString);

            return new RemoteEntities(entities);
        }
    }
}
