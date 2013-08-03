namespace NIHEI.SC4Buddy.View.Plugins
{
    using System.Collections.Generic;

    using NIHEI.SC4Buddy.Entities.Remote;

    public class RemotePluginComparer : IEqualityComparer<RemotePlugin>
    {
        public bool Equals(RemotePlugin x, RemotePlugin y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(RemotePlugin obj)
        {
            return obj.Id;
        }
    }
}