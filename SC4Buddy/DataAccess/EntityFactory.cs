namespace NIHEI.SC4Buddy.DataAccess
{
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

        private Entities CreateEntities()
        {
            var outputLocation = Path.Combine(Path.GetDirectoryName(Application.LocalUserAppDataPath), "Entities");

            var connectionString = string.Format(LocalConnectionString, outputLocation);

            var entities = new DatabaseEntities(connectionString);

            return new Entities(entities);
        }
    }
}
