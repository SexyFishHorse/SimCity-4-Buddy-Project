namespace NIHEI.SC4Buddy.Plugins.Control
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Remote;

    public interface IPluginsController
    {
        ICollection<Plugin> Plugins { get; set; }

        void Add(Plugin plugin);

        void Update(Plugin plugin);

        void Remove(Plugin plugin);

        void UninstallPlugin(Plugin plugin);

        Task<int> UpdateInfoForAllPluginsFromServer(IPluginMatcher pluginMatcher);

        int NumberOfRecognizedPlugins(UserFolder userFolder);

        int RemoveEmptyPlugins();

        void QuarantineFiles(IEnumerable<PluginFile> files);

        void UnquarantineFiles(IEnumerable<PluginFile> files);

        void RemoveFilesFromPlugins(ICollection<string> deletedFilePaths);
    }
}
