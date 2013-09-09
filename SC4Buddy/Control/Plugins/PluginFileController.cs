namespace NIHEI.SC4Buddy.Control.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.IO;
    using System.Linq;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Properties;

    public class PluginFileController
    {
        private readonly IEntities entities;

        public PluginFileController(IEntities entities)
        {
            this.entities = entities;
        }

        public IObjectSet<PluginFile> Files
        {
            get
            {
                return entities.Files;
            }
        }

        public void Delete(PluginFile file, bool save = true)
        {
            entities.Files.DeleteObject(file);
            if (save)
            {
                SaveChanges();
            }
        }

        public void SaveChanges()
        {
            entities.SaveChanges();
        }

        public void DeleteFilesByPath(IEnumerable<string> deletedFiles)
        {
            foreach (
                var pluginFile in deletedFiles
                .Select(GetFileByPath)
                .Where(pluginFile => pluginFile != null))
            {
                Delete(pluginFile);
            }

            SaveChanges();
        }

        public void SetQuarantineStatus(PluginFile file, bool quarantined)
        {
            file.Quarantined = quarantined;
        }

        public void RevertChanges(ICollection<EntityObject> files)
        {
            entities.RevertChanges(files);
        }

        public void MoveFilesBasedOnQuarantineStatus(IList<PluginFile> files)
        {
            MoveQuarantinedFiles(files.Where(x => x.Quarantined.HasValue && x.Quarantined.Value && File.Exists(x.Path)));

            MoveUnquarantinedFiles(files.Where(x => (!x.Quarantined.HasValue || !x.Quarantined.Value) && !File.Exists(x.Path)));
        }

        private void MoveUnquarantinedFiles(IEnumerable<PluginFile> files)
        {
            foreach (var file in files)
            {
                if (file.QuarantinePath != null)
                {
                    File.Copy(file.QuarantinePath, file.Path);
                    File.Delete(file.QuarantinePath);
                }

                file.QuarantinePath = null;
            }
        }

        private void MoveQuarantinedFiles(IEnumerable<PluginFile> files)
        {
            foreach (var file in files)
            {
                var newPath = Path.Combine(Settings.Default.QuarantinedFilesPath, Path.GetRandomFileName());

                File.Copy(file.Path, newPath);
                File.Delete(file.Path);

                file.QuarantinePath = newPath;
            }
        }

        private PluginFile GetFileByPath(string path)
        {
            return Files.FirstOrDefault(x => x.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
        }
    }
}
