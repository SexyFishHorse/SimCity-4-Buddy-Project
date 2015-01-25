namespace NIHEI.SC4Buddy.UserFolders.Control
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NIHEI.SC4Buddy.Configuration;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Model;

    public class UserFolderRepository : IUserFolderRepository
    {
        private readonly IEntities entities;

        public UserFolderRepository(IEntities entities)
        {
            this.entities = entities;
        }

        public IEnumerable<UserFolder> UserFolders
        {
            get
            {
                return entities.UserFolders;
            }
        }

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
                SaveChanges();
            }

            return folder;
        }

        public bool IsGameFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(Settings.Get(Settings.Keys.GameLocation)))
            {
                throw new InvalidOperationException("Game location not set.");
            }

            return Settings.Get(Settings.Keys.GameLocation).Equals(path, StringComparison.OrdinalIgnoreCase);
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
