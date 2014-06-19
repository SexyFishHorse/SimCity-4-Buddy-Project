namespace NIHEI.SC4Buddy.Remote
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Irradiated.Sc4Buddy.ApiClient.Model;

    using Plugin = NIHEI.SC4Buddy.Model.Plugin;
    using RemotePlugin = Irradiated.Sc4Buddy.ApiClient.Model.Plugin;
    using RemotePluginFile = Irradiated.Sc4Buddy.ApiClient.Model.PluginFile;

    public interface IPluginMatcher
    {
        Task<bool> MatchAndUpdateAsync(Plugin plugin);

        Task<IDictionary<PluginFileMetaInfo, IEnumerable<RemotePlugin>>> GetMostLikelyRemotePluginsForFilesAsync(
            IEnumerable<PluginFileMetaInfo> filePathAndChecksum);

        Task<IEnumerable<RemotePlugin>> GetMostLikelyRemotePluginForFileAsync(string filepath, string checksum);
    }
}
