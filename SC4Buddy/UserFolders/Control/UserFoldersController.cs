namespace NIHEI.SC4Buddy.UserFolders.Control
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NIHEI.SC4Buddy.Configuration;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.UserFolders.DataAccess;

    public class UserFoldersController : IUserFoldersController
    {
        private readonly IUserFoldersDataAccess userFoldersDataAccess;

        private readonly IUserFolderController userFolderController;

        public UserFoldersController(IUserFoldersDataAccess userFoldersDataAccess, IUserFolderController userFolderController)
        {
            this.userFoldersDataAccess = userFoldersDataAccess;
            this.userFolderController = userFolderController;

            UserFolders = userFoldersDataAccess.LoadUserFolders();
        }

        public ICollection<UserFolder> UserFolders { get; set; }

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
                    FolderPath = Path.Combine(Settings.Get(Settings.Keys.GameLocation), UserFolder.PluginFolderName)
                };
                Add(folder);
            }

            return folder;
        }

        public void Add(UserFolder userFolder)
        {
            UpdateIsStartupFolder(userFolder);
            UserFolders.Add(userFolder);

            userFolderController.Update(userFolder);
            userFoldersDataAccess.SaveUserFolders(UserFolders);
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

            userFolderController.Update(userFolder);
            userFoldersDataAccess.SaveUserFolders(UserFolders);
        }

        public void Delete(UserFolder userFolder)
        {
            UserFolders.Remove(userFolder);

            userFoldersDataAccess.SaveUserFolders(UserFolders);
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

            var collision = UserFolders
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

        public bool IsNotGameFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(Settings.Get(Settings.Keys.GameLocation)))
            {
                throw new InvalidOperationException("Game location not set.");
            }

            return !Settings.Get(Settings.Keys.GameLocation).Equals(path, StringComparison.OrdinalIgnoreCase);
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

            var collision = UserFolders
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

        public UserFolder GetUserFolderDataByPath(string path)
        {
            return userFolderController.LoadUserFolder(path);
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
