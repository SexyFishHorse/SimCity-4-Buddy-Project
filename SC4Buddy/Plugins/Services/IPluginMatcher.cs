namespace NIHEI.SC4Buddy.Plugins.Services
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using NIHEI.SC4Buddy.Model;
    using Plugin = Asser.Sc4Buddy.Server.Api.V1.Models.Plugin;

    public interface IPluginMatcher
    {
        Plugin GetMostLikelyPluginForGroupOfFiles(IEnumerable<PluginFile> fileInfos);

        IDictionary<PluginFile, Plugin> GetMostLikelyPluginForEachFile(ICollection<PluginFile> files, BackgroundWorker backgroundWorker);

        void ReloadData();
    }
}
