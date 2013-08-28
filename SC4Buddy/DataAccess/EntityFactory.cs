namespace NIHEI.SC4Buddy.DataAccess
{
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
        }
    }
}
