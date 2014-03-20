namespace NIHEI.SC4Buddy.DataAccess
{
    using System;
    using System.Configuration;
    using System.Data.EntityClient;
    using System.IO;

    using NIHEI.SC4Buddy.Entities.Remote;

    public class EntityFactory
    {
        private static EntityFactory instance;

        private Entities entities;

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
                return entities ?? (entities = CreateEntities());
            }
            private set
            {
                entities = value;
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
            var loadEntities =
                new Entities(
                    Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.LocalApplicationData,
                            Environment.SpecialFolderOption.Create),
                        "DataStorage"));

            loadEntities.LoadAllEntitiesFromDisc();

            return loadEntities;
        }
    }
}
