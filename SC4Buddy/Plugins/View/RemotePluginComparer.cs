namespace NIHEI.SC4Buddy.Plugins.View
{
    using System.Collections.Generic;

    using RemotePlugin = Irradiated.Sc4Buddy.ApiClient.Model.Plugin;
    using RemotePluginFile = Irradiated.Sc4Buddy.ApiClient.Model.PluginFile;

    public class RemotePluginComparer : IEqualityComparer<RemotePlugin>
    {
        public bool Equals(RemotePlugin x, RemotePlugin y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(RemotePlugin obj)
        {
            return obj.GetHashCode();
        }
    }
}