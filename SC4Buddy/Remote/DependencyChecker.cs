namespace NIHEI.SC4Buddy.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using NIHEI.SC4Buddy.Model;

    using RemotePlugin = Irradiated.Sc4Buddy.ApiClient.Model.Plugin;

    public class DependencyChecker : IDependencyChecker
    {
        public async Task<IEnumerable<RemotePlugin>> CheckDependenciesAsync(UserFolder userFolder)
        {
            throw new NotImplementedException();
        }
    }
}
