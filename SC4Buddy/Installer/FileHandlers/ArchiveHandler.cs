namespace NIHEI.SC4Buddy.Installer.FileHandlers
{
    using System.Collections.Generic;
    using System.IO;

    using SharpCompress.Archive;
    using SharpCompress.Archive.Rar;

    public class ArchiveHandler : BaseHandler
    {
        public override string RequiredExtension
        {
            get
            {
                return "*";
            }
        }

        public override IEnumerable<FileSystemInfo> ExtractFilesToTemp()
        {
            CheckFileInfoIsSet();
            CreateTempFolder();

            var tempEntries = new List<FileSystemInfo>();

            using (var archive = ArchiveFactory.Open(FileInfo))
            {
                if (archive is RarArchive)
                {
                    var rarArchive = archive as RarArchive;

                    if (rarArchive.IsMultipartVolume() && !rarArchive.IsFirstVolume())
                    {
                        return tempEntries;
                    }
                }

                foreach (var entry in archive.Entries)
                {
                    var tempPath = Path.Combine(TempFolder, entry.FilePath);

                    if (entry.IsDirectory)
                    {
                        Directory.CreateDirectory(tempPath);
                        tempEntries.Add(new DirectoryInfo(tempPath));
                    }
                    else
                    {
                        var directories = Path.GetDirectoryName(tempPath);
                        if (!string.IsNullOrWhiteSpace(directories))
                        {
                            Directory.CreateDirectory(directories);
                        }

                        entry.WriteToFile(tempPath);
                        tempEntries.Add(new FileInfo(tempPath));
                    }
                }
            }

            return tempEntries;
        }
    }
}
