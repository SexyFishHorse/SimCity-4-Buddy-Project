namespace NIHEI.SC4Buddy.DataAccess.Remote
{
    using System;

    using NIHEI.SC4Buddy.Entities.Remote;

    public class RemoteRegistryFactory
    {
        private static readonly RemoteRegistryFactory Instance = new RemoteRegistryFactory();

        private readonly RemotePluginRegistry remotePluginRegistry;

        private readonly RemotePluginFileRegistry remotePluginFileRegistry;

        private readonly UserRegistry userRegistry;

        private readonly AuthorRegistry authorRegistry;

        private RemoteRegistryFactory()
        {
            var entities = new RemoteEntities();

            remotePluginRegistry = new RemotePluginRegistry(entities);

            remotePluginFileRegistry = new RemotePluginFileRegistry(entities);

            userRegistry = new UserRegistry(entities);

            authorRegistry = new AuthorRegistry(entities);
        }

        [Obsolete("Use entity controllers directly")]
        public static RemotePluginRegistry RemotePluginRegistry
        {
            get
            {
                return Instance.remotePluginRegistry;
            }
        }

        [Obsolete("Use entity controllers directly")]
        public static RemotePluginFileRegistry RemotePluginFileRegistry
        {
            get
            {
                return Instance.remotePluginFileRegistry;
            }
        }

        [Obsolete("Use entity controllers directly")]
        public static UserRegistry UserRegistry
        {
            get
            {
                return Instance.userRegistry;
            }
        }

        [Obsolete("Use entity controllers directly")]
        public static AuthorRegistry AuthorRegistry
        {
            get
            {
                return Instance.authorRegistry;
            }
        }
    }
}
