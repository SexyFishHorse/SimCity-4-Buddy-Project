namespace NIHEI.SC4Buddy.DataAccess
{
    using System;
    using System.Configuration;
    using System.Data.EntityClient;

    using NIHEI.SC4Buddy.Entities.Remote;

    public class EntityFactory
    {
        private static EntityFactory instance;

        private RemoteEntities remoteEntities;

        private EntityFactory()
        {
            RemoteEntities = CreateRemoteEntities();

            Entities = CreateEntities();
        }

        public static EntityFactory Instance
        {
            get
            {
                return instance ?? (instance = new EntityFactory());
            }
        }

        public Entities Entities
        {
            get
            {
                throw new NotImplementedException();
            }
            private set
            {
                throw new NotImplementedException();
            }
        }

        public RemoteEntities RemoteEntities
        {
            get
            {
                if (remoteEntities == null || remoteEntities.Disposed)
                {
                    remoteEntities = CreateRemoteEntities();
                }

                return remoteEntities;
            }
            private set
            {
                remoteEntities = value;
            }
        }

        private RemoteEntities CreateRemoteEntities()
        {
            var originalConnectionString = ConfigurationManager.ConnectionStrings["RemoteDatabaseEntities"].ConnectionString;

            var entityBuilder = new EntityConnectionStringBuilder(originalConnectionString);
            var providerConnectionString = entityBuilder.ProviderConnectionString;
            providerConnectionString += ";password=_VslefXPl5Tg8pBcSYzI";

            entityBuilder.ProviderConnectionString = providerConnectionString;

            var databaseEntities = new RemoteDatabaseEntities(entityBuilder.ConnectionString);

            return new RemoteEntities(databaseEntities);
        }

        private Entities CreateEntities()
        {
            throw new NotImplementedException();
        }
    }
}
