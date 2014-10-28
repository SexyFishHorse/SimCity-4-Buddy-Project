namespace NIHEI.SC4Buddy.Control.UserFolders
{
    using System.Threading.Tasks;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Remote;

    public interface IUserFolderController
    {
        void SaveChanges();

        void UninstallPlugin(Plugin selectedPlugin);

        Task<int> UpdateInfoForAllPluginsFromServer(IPluginMatcher pluginMatcher);

        int NumberOfRecognizedPlugins(UserFolder userFolder);
    }
}
