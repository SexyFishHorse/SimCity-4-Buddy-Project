namespace NIHEI.SC4Buddy.Control.UserFolders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.VisualBasic.FileIO;

    using NIHEI.SC4Buddy.Control.Plugins;
    using NIHEI.SC4Buddy.Control.Remote;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Properties;
    using NIHEI.SC4Buddy.Remote;

    public class UserFolderController
    {
        private readonly IEntities entities;

        private readonly PluginFileController pluginFileController;

        private readonly PluginController pluginController;

        public UserFolderController(IEntities entities)
        {
            this.entities = entities;

            pluginFileController = new PluginFileController(EntityFactory.Instance.Entities);
            pluginController = new PluginController(EntityFactory.Instance.Entities);
        }

        public IEnumerable<UserFolder> UserFolders
        {
            get
            {
                return entities.UserFolders;
            }
        }

        public static bool IsDamnFile(UserFolder userFolder, string path)
        {
            var relativePath = path.Replace(userFolder.PluginFolderPath, string.Empty);

            return relativePath.StartsWith(@"\DAMN", StringComparison.OrdinalIgnoreCase)
                   && (relativePath.EndsWith("placeholder", StringComparison.OrdinalIgnoreCase)
                       || relativePath.EndsWith("DAMN-Indexer.cmd"));
        }

        public static bool IsBackgroundImage(string entity, UserFolder userFolder)
        {
            if (!userFolder.IsMainFolder)
            {
                return false;
            }

            var validFilenames = new[]
                                     {
                                         "Background3D0.png", "Background3D1.png", "Background3D2.png",
                                         "Background3D3.png", "Background3D4.png"
                                     };

            return validFilenames.Any(entity.EndsWith);
        }

        /// <summary>
        /// Validates that the specified path is not empty or a whitespace 
        /// and that the path exist on the local machine.
        /// It also checks if the path is already in use in 
        /// </summary>
        /// <param name="path">The path to validate.</param>
        /// <param name="currentId">The id of the object to skip when checking for uniqueness.</param>
        /// <returns>TRUE if the path complies with the above rules.</returns>
        public bool ValidatePath(string path, Guid currentId)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            if (!Directory.Exists(path))
            {
                return false;
            }

            var collision = entities.UserFolders
                .FirstOrDefault(x => x.FolderPath.Equals(path, StringComparison.OrdinalIgnoreCase));

            if (currentId == Guid.Empty)
            {
                return collision == null;
            }

            if (collision != null)
            {
                return collision.Id == currentId;
            }

            return true;
        }

        /// <summary>
        /// Validates that the specified alias is not empty or a whitespace
        /// and that the alias is not already in use.
        /// </summary>
        /// <param name="alias">The alias to validate.</param>
        /// <param name="currentId">The id of the object to skip when checking for uniqueness.</param>
        /// <returns>TRUE if the alias complies with the above rules.</returns>
        public bool ValidateAlias(string alias, Guid currentId)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                return false;
            }

            var collision = entities.UserFolders
                .FirstOrDefault(x => x.Alias.Equals(alias, StringComparison.OrdinalIgnoreCase));

            if (currentId == Guid.Empty)
            {
                return collision == null;
            }

            if (collision != null)
            {
                return collision.Id == currentId;
            }

            return true;
        }

        public void Delete(UserFolder userFolder)
        {
            entities.UserFolders.Remove(userFolder);
        }

        public void Add(UserFolder userFolder)
        {
            UpdateIsStartupFolder(userFolder);

            entities.UserFolders.Add(userFolder);
        }

        public void Update(UserFolder userFolder)
        {
            if (userFolder.IsStartupFolder)
            {
                foreach (var folder in UserFolders.Where(x => x.IsStartupFolder && x.Id != userFolder.Id))
                {
                    folder.IsStartupFolder = false;
                }
            }

            entities.SaveChanges();
        }

        public void SaveChanges()
        {
            entities.SaveChanges();
        }

        public bool IsNotGameFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.GameLocation))
            {
                throw new InvalidOperationException("Game location not set.");
            }

            return !Settings.Default.GameLocation.Equals(path, StringComparison.OrdinalIgnoreCase);
        }

        public void UninstallPlugin(Plugin selectedPlugin)
        {
            var files = new PluginFile[selectedPlugin.PluginFiles.Count];
            selectedPlugin.PluginFiles.CopyTo(files, 0);
            foreach (var file in files)
            {
                if (File.Exists(file.Path))
                {
                    FileSystem.DeleteFile(file.Path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }

                pluginFileController.Delete(file);
            }

            pluginController.Delete(selectedPlugin);
        }

        public int UpdateInfoForAllPluginsFromServer()
        {
            var plugins = pluginController.Plugins;

            var matcher = new PluginMatcher(
                pluginController, new RemotePluginFileController(EntityFactory.Instance.RemoteEntities));

            return plugins.Count(matcher.MatchAndUpdate);
        }

        public int NumberOfRecognizedPlugins(UserFolder userFolder)
        {
            return pluginController.Plugins.Count(x => x.RemotePluginId > 0 && x.UserFolder.Id == userFolder.Id);
        }

        public UserFolder GetMainUserFolder()
        {
            var folder = UserFolders.FirstOrDefault(x => x.IsMainFolder);

            if (folder == null)
            {
                folder = new UserFolder
                             {
                                 Id = Guid.NewGuid(),
                                 Alias = "Main user folder",
                                 IsMainFolder = true,
                                 FolderPath = Path.Combine(Settings.Default.GameLocation, UserFolder.PluginFolderName)
                             };
                Add(folder);
                SaveChanges();
            }

            return folder;
        }

        private void UpdateIsStartupFolder(UserFolder userFolder)
        {
            if (userFolder.IsMainFolder)
            {
                userFolder.IsStartupFolder = false;
            }

            if (!userFolder.IsStartupFolder)
            {
                return;
            }

            foreach (var folder in UserFolders.Where(x => x.IsStartupFolder && x.Id != userFolder.Id))
            {
                folder.IsStartupFolder = false;
            }
        }
    }
}
