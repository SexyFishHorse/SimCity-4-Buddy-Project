namespace NIHEI.SC4Buddy.Plugins.Control
{
    using System.Collections.Generic;
    using NIHEI.SC4Buddy.Model;

    public interface IPluginFileController
    {
        IEnumerable<PluginFile> Files { get; }

        void Delete(PluginFile file, bool save = true);

        void SaveChanges();

        void DeleteFilesByPath(IEnumerable<string> deletedFiles);

        void RevertChanges(IEnumerable<ModelBase> files);

        void QuarantineFiles(IEnumerable<PluginFile> files);

        void UnquarantineFiles(IEnumerable<PluginFile> files);
    }
}