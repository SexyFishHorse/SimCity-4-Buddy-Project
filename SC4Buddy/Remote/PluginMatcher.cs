namespace NIHEI.SC4Buddy.Remote
{
    using System;
    using System.IO;

    using Irradiated.Sc4Buddy.ApiClient;

    using NIHEI.SC4Buddy.Model;

    using RemotePlugin = Irradiated.Sc4Buddy.ApiClient.Model.Plugin;
    using RemotePluginFile = Irradiated.Sc4Buddy.ApiClient.Model.PluginFile;

    public class PluginMatcher : IPluginMatcher
    {
        private ISc4BuddyApiClient client;

        public PluginMatcher(ISc4BuddyApiClient client)
        {
            this.client = client;
        }

        public bool MatchAndUpdate(Plugin plugin)
        {
            throw new NotImplementedException();
        }

        public RemotePlugin GetMostLikelyRemotePluginForFile(string filePath, string checksum)
        {
            throw new NotImplementedException();
        }
    }
}
