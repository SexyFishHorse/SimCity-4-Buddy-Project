namespace NIHEI.SC4Buddy.DataAccess
{
    using System;
    using System.IO;

    public class EntityFactory
    {
        private static EntityFactory instance;

        private Entities entities;

        private EntityFactory()
        {
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

        private Entities CreateEntities()
        {
            var loadEntities =
                new Entities(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "Irradiated Games",
                        "SimCity 4 Buddy",
                        "DataStorage"));

            loadEntities.LoadAllEntitiesFromDisc();

            return loadEntities;
        }
    }
}
