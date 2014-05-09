namespace NIHEI.SC4Buddy.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using NIHEI.SC4Buddy.Model;

    using RemotePlugin = Irradiated.Sc4Buddy.ApiClient.Model.Plugin;
    using RemotePluginFile = Irradiated.Sc4Buddy.ApiClient.Model.PluginFile;

    public interface IPluginMatcher
    {
        Task<bool> MatchAndUpdateAsync(Plugin plugin);

        Task<IDictionary<string, IEnumerable<RemotePlugin>>> GetMostLikelyRemotePluginsForFilesAsync(
            IEnumerable<Tuple<string, string>> filePathAndChecksum);

        Task<IEnumerable<RemotePlugin>> GetMostLikelyRemotePluginForFileAsync(string filepath, string checksum);
    }
}
