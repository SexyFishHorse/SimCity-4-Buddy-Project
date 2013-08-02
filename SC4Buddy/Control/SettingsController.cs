namespace NIHEI.SC4Buddy.Control
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.Win32;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Properties;

    public class SettingsController
    {
        private readonly string[] regKeys = new[]
                              {
                                  @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                                  @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
                              };

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

        public string SearchForGameLocation()
        {
            var match = false;

            var gamePath = string.Empty;

            foreach (var regKey in regKeys)
            {
                using (var key = Registry.LocalMachine.OpenSubKey(regKey))
                {
                    if (key == null)
                    {
                        continue;
                    }

                    foreach (var subKeyName in key.GetSubKeyNames())
                    {
                        using (var subKey = key.OpenSubKey(subKeyName))
                        {
                            if (subKey == null || string.IsNullOrWhiteSpace((string)subKey.GetValue("DisplayName")))
                            {
                                continue;
                            }

                            var name = (string)subKey.GetValue("DisplayName");
                            var path = (string)subKey.GetValue("InstallLocation");

                            if (!name.StartsWith("SimCity 4") || !ValidateGameLocationPath(path))
                            {
                                continue;
                            }

                            match = true;
                            gamePath = path;

                            break;
                        }
                    }

                    if (match)
                    {
                        break;
                    }
                }
            }

            return gamePath;
        }

        public List<string> GetInstalledLanguages()
        {
            var dirs = Directory.EnumerateDirectories(Settings.Default.GameLocation, "*", SearchOption.TopDirectoryOnly);

            var languages =
                dirs.Select(dir => new { dir, files = Directory.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly) })
                    .Where(@t => @t.files.Any(file => file.EndsWith("SimCityLocale.dat", StringComparison.OrdinalIgnoreCase)))
                    .Select(@t => new DirectoryInfo(@t.dir).Name)
                    .ToList();

            return languages;
        }


    }
}
