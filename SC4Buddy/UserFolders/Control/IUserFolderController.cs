namespace NIHEI.SC4Buddy.UserFolders.Control
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Remote;

    public interface IUserFolderController
    {
        IEnumerable<UserFolder> UserFolders { get; }

        void Update(UserFolder userFolder);

        void SaveChanges();

        void UninstallPlugin(Plugin selectedPlugin);

        Task<int> UpdateInfoForAllPluginsFromServer(IPluginMatcher pluginMatcher);

        int NumberOfRecognizedPlugins(UserFolder userFolder);
    }
}