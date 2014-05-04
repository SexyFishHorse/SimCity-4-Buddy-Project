namespace NIHEI.SC4Buddy.Remote
{
    using System.Collections.Generic;

    using NIHEI.SC4Buddy.Model;

    using RemotePlugin = Irradiated.Sc4Buddy.ApiClient.Model.Plugin;
    using RemotePluginFile = Irradiated.Sc4Buddy.ApiClient.Model.PluginFile;

    public interface IDependencyChecker
    {
        List<RemotePlugin> CheckDependencies(UserFolder userFolder);
    }
}