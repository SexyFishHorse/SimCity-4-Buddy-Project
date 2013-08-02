namespace NIHEI.SC4Buddy.Control
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Win32;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Properties;

    using log4net;

    public class SettingsController : ISettingsController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
            Log.Error(string.Format("Validating game path: {0}", path));
            if (string.IsNullOrWhiteSpace(path))
            {
                Log.Error("Game location is empty");
                throw new ArgumentException(@"Path cannot be null or empty", "path");
            }

            return File.Exists(Path.Combine(path, @"Apps\SimCity 4.exe"));
        }

        public void UpdateMainFolder()
        {
            Log.Info("Updating main folder");

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
            Log.Info("Searching for game location");
            var match = false;

            var gamePath = string.Empty;

            foreach (var regKey in regKeys)
            {
                Log.Info(string.Format("Checking key: {0}", regKey));
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

                            if (!name.StartsWith("SimCity 4") || string.IsNullOrWhiteSpace(path) || !ValidateGameLocationPath(path))
                            {
                                continue;
                            }

                            Log.Info(string.Format("Found a match. Name: {0}, Path: {1}", name, path));

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
                dirs.Select(
                    dir => new { dir, files = Directory.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly) })
                    .Where(
                        @t =>
                        @t.files.Any(file => file.EndsWith("SimCityLocale.dat", StringComparison.OrdinalIgnoreCase)))
                    .Select(@t => new DirectoryInfo(@t.dir).Name)
                    .ToList();

            return languages;
        }

        public List<Bitmap> GetWallpapers()
        {
            return new List<Bitmap>
                       {
                           Resources.Wallpaper1,
                           Resources.Wallpaper2,
                           Resources.Wallpaper3,
                           Resources.Wallpaper4,
                           Resources.Wallpaper5,
                           Resources.Wallpaper6,
                           Resources.Wallpaper7,
                           Resources.Wallpaper8,
                           Resources.Wallpaper9,
                           Resources.Wallpaper10,
                           Resources.Wallpaper11,
                           Resources.Wallpaper12,
                           Resources.Wallpaper13
                       };
        }
    }
}
