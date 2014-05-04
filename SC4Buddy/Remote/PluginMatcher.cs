namespace NIHEI.SC4Buddy.Remote
{
    using RemotePlugin = Irradiated.Sc4Buddy.ApiClient.Model.Plugin;
    using RemotePluginFile = Irradiated.Sc4Buddy.ApiClient.Model.PluginFile;

    public class PluginMatcher : IPluginMatcher
    {
        public bool MatchAndUpdate(Model.Plugin plugin)
        {
            throw new System.NotImplementedException();
        }

        public RemotePlugin GetMostLikelyRemotePluginForFile(string filePath, string checksum)
        {
            throw new System.NotImplementedException();
        }
    }
}
