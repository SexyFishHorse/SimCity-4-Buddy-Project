namespace NIHEI.SC4Buddy.Plugins.Control
{
    using System.Threading.Tasks;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Remote;

    public interface IPluginsController
    {
        void UninstallPlugin(Plugin selectedPlugin);

        Task<int> UpdateInfoForAllPluginsFromServer(IPluginMatcher pluginMatcher);

        int NumberOfRecognizedPlugins(UserFolder userFolder);
    }
}