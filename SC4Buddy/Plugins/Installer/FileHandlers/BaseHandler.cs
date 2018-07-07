namespace Nihei.SC4Buddy.Plugins.Installer.FileHandlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Nihei.Common.IO;
    using Nihei.SC4Buddy.Model;

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

                if (RequiredExtension != "*" && value.Extension != RequiredExtension)
                {
                    throw new ArgumentException(string.Format("FileInfo must point to a {0} file.", RequiredExtension));
                }

                fileInfo = value;
            }
        }

        public string TempFolder { get; set; }

        public static bool IsPluginFile(string entry)
        {
            return
                PluginFileExtensions.Any(
                    extension => new FileInfo(entry).Extension.Equals(extension, StringComparison.OrdinalIgnoreCase));
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