namespace NIHEI.SC4Buddy.DataAccess
{
    using System;
    using System.Configuration;
    using System.Data.EntityClient;
    using System.IO;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Entities.Remote;

    public class EntityFactory
    {
        private const string LocalConnectionString =
            @"metadata=res://*/DatabaseEntities.csdl|res://*/DatabaseEntities.ssdl|res://*/DatabaseEntities.msl;provider=System.Data.SqlServerCe.4.0;provider connection string='data source={0}\Database.sdf'";

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
                if (entities == null || entities.Disposed)
                {
                    entities = CreateEntities();
                }

                return entities;
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
            var appDataPath = Path.GetDirectoryName(Application.LocalUserAppDataPath);

            if (appDataPath == null || string.IsNullOrWhiteSpace(appDataPath))
            {
                throw new InvalidOperationException("Unable to locate local user app data path.");
            }

            var outputLocation = Path.Combine(appDataPath, "Entities");

            var connectionString = string.Format(LocalConnectionString, outputLocation);

            var databaseEntities = new DatabaseEntities(connectionString);

            return new Entities(databaseEntities);
        }
    }
}
