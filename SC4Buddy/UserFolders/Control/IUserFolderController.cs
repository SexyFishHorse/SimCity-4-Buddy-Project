namespace NIHEI.SC4Buddy.UserFolders.Control
{
    using System.Threading.Tasks;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Remote;

    public interface IUserFolderController
    {
        UserFolder UserFolder { get; set; }

        void UninstallPlugin(Plugin selectedPlugin);

        Task<int> UpdateInfoForAllPluginsFromServer(IPluginMatcher pluginMatcher);

        int NumberOfRecognizedPlugins(UserFolder userFolder);

        void Save();
    }
}