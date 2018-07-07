namespace Nihei.SC4Buddy.Application.Utilities
{
    using System;
    using System.IO;

    public static class FileSystemLocationsUtil
    {
        public static string LocalApplicationDirectory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Irradiated Games", "SimCity 4 Buddy");
            }
        }

        public static string LogFilesDirectory
        {
            get
            {
                return Path.Combine(LocalApplicationDirectory, "Logs");
            }
        }

        public static string LocalApplicationDataDirectory
        {
            get
            {
                return Path.Combine(LocalApplicationDirectory, "DataStorage");
            }
        }
    }
}
