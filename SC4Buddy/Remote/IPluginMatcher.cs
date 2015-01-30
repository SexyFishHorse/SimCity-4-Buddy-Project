namespace NIHEI.SC4Buddy.Remote
{
    using System.Collections.Generic;
    using NIHEI.SC4Buddy.Model;
    using Plugin = Asser.Sc4Buddy.Server.Api.V1.Models.Plugin;

    public interface IPluginMatcher
    {
        Plugin GetMostLikelyPluginForGroupOfFiles(IEnumerable<PluginFile> fileInfos);
    }
}
