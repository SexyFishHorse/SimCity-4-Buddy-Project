namespace NIHEI.SC4Buddy.Installer.FileHandlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NIHEI.Common.IO;
    using NIHEI.SC4Buddy.Entities;

    public abstract class BaseHandler
    {
        public static readonly string[] PluginFileExtensions = new[] { ".dat", ".SC4Lot", ".SC4Desc", ".SC4Model", ".sav", ".dll" };

        private FileInfo fileInfo;

        public abstract string RequiredExtension { get; }

        public FileInfo FileInfo
        {
            get
            {
                return fileInfo;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (!value.Exists)
                {
                    throw new FileNotFoundException("FileInfo does not point to an existing file.");
                }

                fileInfo = value;
            }
        }

        public string TempFolder { get; set; }

        public static bool IsPluginFile(string entry)
        {
            var match = false;
            foreach (var extension in PluginFileExtensions.Where(extension => new FileInfo(entry).Extension.Equals(extension, StringComparison.OrdinalIgnoreCase)))
            {
                match = true;
            }

            return match;
        }

        public IEnumerable<PluginFile> MoveToPluginFolder(UserFolder userFolder)
        {
            if (userFolder == null)
            {
                throw new ArgumentNullException("userFolder", @"UserFolder may not be null.");
            }

            if (
                TempFolder == null
                || !Directory.Exists(TempFolder)
                || Directory.GetFileSystemEntries(TempFolder).Length == 0)
            {
                throw new InvalidOperationException("The archive has not been extracted to the temp folder.");
            }

            var newPath = userFolder.PluginFolderPath;

            var entries = Directory.GetFileSystemEntries(TempFolder, "*", SearchOption.AllDirectories);

            for (var i = 0; i < entries.Length; i++)
            {
                entries[i] = entries[i].Replace(TempFolder, newPath);
            }

            FileUtility.CopyFolder(new DirectoryInfo(TempFolder), new DirectoryInfo(newPath));
            FileUtility.DeleteFolder(new DirectoryInfo(TempFolder));

            return
                entries.Where(File.Exists)
                       .Select(entry => new { entry, checksum = Md5ChecksumUtility.CalculateChecksum(entry).ToHex() })
                       .Select(@t => new PluginFile { Path = @t.entry, Checksum = @t.checksum });
        }

        public abstract IEnumerable<FileSystemInfo> ExtractFilesToTemp();

        protected void CreateTempFolder()
        {
            if (Directory.Exists(TempFolder))
            {
                FileUtility.DeleteFolder(TempFolder);
            }

            Directory.CreateDirectory(TempFolder);
        }

        protected void CheckFileInfoIsSet()
        {
            if (FileInfo == null)
            {
                throw new InvalidOperationException("FileInfo is not set.");
            }
        }
    }
}