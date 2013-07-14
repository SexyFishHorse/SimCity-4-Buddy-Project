namespace NIHEI.SC4Buddy.DataAccess.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NIHEI.SC4Buddy.Entities;

    public interface IPluginFileRegistry
    {
        ICollection<PluginFile> Files { get; }

        void Add(PluginFile file);

        void Update(PluginFile file);

        void Delete(PluginFile file);
    }

    public class PluginFileRegistry : IPluginFileRegistry
    {
        private readonly DatabaseEntities entities;

        private bool saveImmediately = true;

        public PluginFileRegistry(DatabaseEntities databaseEntities)
        {
            entities = databaseEntities;
        }

        public ICollection<PluginFile> Files
        {
            get
            {
                return entities.PluginFiles.ToList();
            }
        }

        public void Add(PluginFile file)
        {
            entities.PluginFiles.AddObject(file);
            if (saveImmediately)
            {
                entities.SaveChanges();
            }
        }

        public void Update(PluginFile file)
        {
            if (file.Id < 1)
            {
                throw new ArgumentException(@"File must have an id", "file");
            }

            if (entities.PluginFiles.Any(x => x.Id == file.Id))
            {
                entities.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException(
                    "File not in collection (did you change the id or forgot to add it first?)");
            }
        }

        public void Delete(PluginFile file)
        {
            if (!entities.PluginFiles.Any(x => x.Id == file.Id))
            {
                throw new ArgumentException(@"Not contained in the entity set.", "file");
            }

            entities.PluginFiles.DeleteObject(file);
            if (saveImmediately)
            {
                entities.SaveChanges();
            }
        }

        public PluginFile GetFileByPath(string path)
        {
            return Files.FirstOrDefault(x => x.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
        }

        public void DeleteFilesByPath(ICollection<string> deletedFiles)
        {
            BeginUpdate();
            foreach (
                var pluginFile in deletedFiles
                .Select(GetFileByPath)
                .Where(pluginFile => pluginFile != null))
            {
                Delete(pluginFile);
            }

            EndUpdate();
        }

        /// <summary>
        /// Prevents the registry from updating the database until EndUpdate is called.
        /// </summary>
        public void BeginUpdate()
        {
            saveImmediately = false;
        }

        /// <summary>
        /// Writes all changes to the database and resumes immediate saves.
        /// </summary>
        public void EndUpdate()
        {
            saveImmediately = true;
            entities.SaveChanges();
        }
    }
}
