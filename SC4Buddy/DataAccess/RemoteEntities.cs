namespace NIHEI.SC4Buddy.DataAccess
{
    using System;
    using System.Collections.Generic;

    using SC4Buddy.Entities.Remote;

    public class RemoteEntities : IRemoteEntities
    {
        public RemoteEntities()
        {
            Disposed = false;
        }

        public bool Disposed { get; private set; }

        public ICollection<Author> Authors
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICollection<RemotePlugin> Plugins
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICollection<RemotePluginFile> PluginFiles
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICollection<User> Users
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void RevertChanges(object entityObject)
        {
            throw new NotImplementedException();
        }

        public void RevertChanges(IEnumerable<object> entityCollection)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Disposed = true;
        }
    }
}
