namespace NIHEI.SC4Buddy.Remote
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using NIHEI.SC4Buddy.Model;

    using RemotePlugin = Irradiated.Sc4Buddy.ApiClient.Model.Plugin;
    using RemotePluginFile = Irradiated.Sc4Buddy.ApiClient.Model.PluginFile;

    public interface IDependencyChecker
    {
        Task<IEnumerable<RemotePlugin>> CheckDependenciesAsync(UserFolder userFolder);
    }
}