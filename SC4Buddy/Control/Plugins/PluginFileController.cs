namespace NIHEI.SC4Buddy.Control.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NIHEI.SC4Buddy.Configuration;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Model;

    public class PluginFileController
    {
        private readonly IEntities entities;

        public PluginFileController(IEntities entities)
        {
            this.entities = entities;
        }

        public IEnumerable<PluginFile> Files
        {
            get
            {
                return entities.Files;
            }
        }

        public void Delete(PluginFile file, bool save = true)
        {
            entities.Files.Remove(file);
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

        public void RevertChanges(IEnumerable<ModelBase> files)
        {
            entities.RevertChanges(files);
        }

        public void QuarantineFiles(IEnumerable<PluginFile> files)
        {
            foreach (var file in files.Where(x => File.Exists(x.Path)))
            {
                var newPath = Path.Combine(Settings.Get(Settings.Keys.QuarantinedFiles), Path.GetRandomFileName());
                Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                File.Copy(file.Path, newPath);
                File.Delete(file.Path);

                file.QuarantinedFile = new QuarantinedFile { PluginFile = file, QuarantinedPath = newPath };
            }
        }

        public void UnquarantineFiles(IEnumerable<PluginFile> files)
        {
            foreach (var file in files.Where(x => x.QuarantinedFile != null && File.Exists(x.QuarantinedFile.QuarantinedPath)))
            {
                if (file.QuarantinedFile != null)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(file.Path));
                    File.Copy(file.QuarantinedFile.QuarantinedPath, file.Path);
                    File.Delete(file.QuarantinedFile.QuarantinedPath);
                }

                file.QuarantinedFile = null;
            }
        }

        private PluginFile GetFileByPath(string path)
        {
            return Files.FirstOrDefault(x => x.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
        }
    }
}
