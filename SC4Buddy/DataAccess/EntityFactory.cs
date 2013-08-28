namespace NIHEI.SC4Buddy.DataAccess
{
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
        }

        public Entities Entities { get; private set; }

        public RemoteEntities RemoteEntities { get; private set; }
        }
    }
}
