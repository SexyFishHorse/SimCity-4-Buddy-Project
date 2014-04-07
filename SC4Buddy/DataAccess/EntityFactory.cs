namespace NIHEI.SC4Buddy.DataAccess
{
    using System;
    using System.IO;

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
            return new RemoteEntities();
        }

        private Entities CreateEntities()
        {
            var loadEntities =
                new Entities(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "DataStorage"));

            loadEntities.LoadAllEntitiesFromDisc();

            return loadEntities;
        }
    }
}
