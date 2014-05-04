namespace NIHEI.SC4Buddy.Remote
{
    using NIHEI.SC4Buddy.Model;

    using RemotePlugin = Irradiated.Sc4Buddy.ApiClient.Model.Plugin;
    using RemotePluginFile = Irradiated.Sc4Buddy.ApiClient.Model.PluginFile;

    public interface IPluginMatcher
    {
        bool MatchAndUpdate(Plugin plugin);

        RemotePlugin GetMostLikelyRemotePluginForFile(string filePath, string checksum);
    }
}