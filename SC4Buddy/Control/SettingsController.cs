namespace NIHEI.SC4Buddy.Control
{
    using System;
    using System.IO;
    using System.Linq;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Properties;

    public class SettingsController
    {
        private readonly IUserFolderRegistry userFolderRegistry;

        public SettingsController(IUserFolderRegistry userFolderRegistry)
        {
            this.userFolderRegistry = userFolderRegistry;
        }

        public bool ValidateGameLocationPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(@"Path cannot be null or empty", "path");
            }

            return File.Exists(Path.Combine(path, @"Apps\SimCity 4.exe"));
        }

        public void UpdateMainFolder()
        {
            var folder = userFolderRegistry.UserFolders.FirstOrDefault(x => x.Id == 1);
            if (folder == null)
            {
                throw new InvalidOperationException("Main plugin folder has been deleted from the database.");
            }

            folder.Path = Settings.Default.GameLocation;
            folder.Alias = LocalizationStrings.GameUserFolderName;
            userFolderRegistry.Update(folder);
        }
    }
}
