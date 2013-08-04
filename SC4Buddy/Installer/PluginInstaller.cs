namespace NIHEI.SC4Buddy.Installer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Installer.FileHandlers;

    public class PluginInstaller
    {
        public IEnumerable<FileSystemInfo> PluginFiles { get; set; }

        public IEnumerable<FileInfo> Executables { get; set; }

        public IEnumerable<FileInfo> ReadmeFiles { get; set; }

        public IEnumerable<PluginFile> InstalledFiles { get; set; }

        private BaseHandler FileHandler { get; set; }

        public void ExtractToTempFolder(FileInfo fileInfo, string tempFolderPath)
        {
            if (!fileInfo.Exists)
            {
                throw new ArgumentException("FileInfo must point to an existing file.");
            }

            switch (fileInfo.Extension.ToUpper())
            {
                case ".DAT":
                    FileHandler = new DatHandler();
                    break;
                case ".ZIP":
                    FileHandler = new ArchiveHandler();
                    break;
                case ".RAR":
                    FileHandler = new ArchiveHandler();
                    break;
                default:
                    throw new ArgumentException(@"FileInfo must point to either a dat, rar or zip file.", "fileInfo");
            }

            FileHandler.TempFolder = tempFolderPath;
            FileHandler.FileInfo = fileInfo;

            var tempFiles = FileHandler.ExtractFilesToTemp().ToList();
            ReadmeFiles = GetReadmeFiles(tempFiles);
            Executables = GetExecutables(tempFiles);
            PluginFiles = GetPluginFiles(tempFiles);
        }

        public void MoveToUserFolder(UserFolder userFolder)
        {
            InstalledFiles = FileHandler.MoveToPluginFolder(userFolder);
        }

        private static IEnumerable<FileSystemInfo> GetPluginFiles(IEnumerable<FileSystemInfo> tempFiles)
        {
            var pluginExtensions = new[] { ".dat", ".SC4Lot", ".SC4Desc", ".SC4Model" };
            return
                tempFiles.SelectMany(info => pluginExtensions, (info, extension) => new { info, extension })
                         .Where(
                             @t =>
                             @t.info is DirectoryInfo
                             || @t.info.Extension.Equals(@t.extension, StringComparison.OrdinalIgnoreCase))
                         .Select(@t => @t.info)
                         .ToList();
        }

        private static IEnumerable<FileInfo> GetExecutables(IEnumerable<FileSystemInfo> tempFiles)
        {
            return
                tempFiles.Where(x => x is FileInfo && x.Extension.Equals(".exe", StringComparison.OrdinalIgnoreCase))
                         .Cast<FileInfo>()
                         .ToList();
        }

        private static IEnumerable<FileInfo> GetReadmeFiles(IEnumerable<FileSystemInfo> tempFiles)
        {
            var readmeExtensions = new[] { ".html", ".htm", ".mht", ".pdf", ".txt", ".rtf", ".doc", ".docx", "odt" };
            var nonReadmeFilenames = new[] { "CLEANITOL", "REMOVELIST" };

            var readmeFiles = new List<FileInfo>();
            foreach (
                FileInfo file in
                    tempFiles.Cast<FileInfo>()
                             .Where(file => !nonReadmeFilenames.Any(x => file.Name.ToUpper().Contains(x))))
            {
                readmeFiles.AddRange(
                    readmeExtensions.Where(
                        readmeExtension => file.Extension.Equals(readmeExtension, StringComparison.OrdinalIgnoreCase))
                                    .Select(readmeExtension => file));
            }

            return readmeFiles;
        }
    }
}
