namespace NIHEI.SC4Buddy.Remote
{
    using System.Collections.Generic;

    using Irradiated.Sc4Buddy.ApiClient.Model;

    using PluginFile = NIHEI.SC4Buddy.Model.PluginFile;

    public class PluginMatcher : IPluginMatcher
    {
        public IEnumerable<Plugin> GetPossibleRemotePlugins(IList<PluginFile> files)
        {
            throw new System.NotImplementedException();
        }

        public Plugin GetMostLikelyRemotePlugin(IList<PluginFile> files)
        {
            throw new System.NotImplementedException();
        }

        public bool MatchAndUpdate(Model.Plugin plugin)
        {
            throw new System.NotImplementedException();
        }

        public Plugin GetMostLikelyRemotePluginForFile(string filePath, string checksum)
        {
            throw new System.NotImplementedException();
        }
    }
}
