namespace NIHEI.SC4Buddy.Control
{
    using System;
    using System.IO;

    public class SettingsController
    {
        public bool ValidateGameLocationPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(@"Path cannot be null or empty", "path");
            }

            return File.Exists(Path.Combine(path, @"Apps\SimCity 4.exe"));
        }
    }
}
