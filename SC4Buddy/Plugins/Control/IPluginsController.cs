namespace NIHEI.SC4Buddy.Plugins.Control
{
    using System.Collections.Generic;
    using NIHEI.SC4Buddy.Model;

    public interface IPluginsController
    {
        ICollection<Plugin> Plugins { get; set; }

        void Add(Plugin plugin);

        void Update(Plugin plugin);

        void Remove(Plugin plugin);

        void UninstallPlugin(Plugin plugin);

        int IdentifyNewPlugins();

        int NumberOfRecognizedPlugins(UserFolder userFolder);

        int RemoveEmptyPlugins();

        void QuarantineFiles(IEnumerable<PluginFile> files);

        void UnquarantineFiles(IEnumerable<PluginFile> files);

        void RemoveFilesFromPlugins(ICollection<string> deletedFilePaths);

        void ReloadPlugins();
    }
}
