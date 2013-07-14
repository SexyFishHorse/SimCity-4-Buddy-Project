namespace NIHEI.SC4Buddy.Installer.FileHandlers
{
    using System.Collections.Generic;
    using System.IO;

    using SharpCompress.Reader;
    using SharpCompress.Reader.Rar;

    public class RarHandler : BaseHandler
    {
        public override string RequiredExtension
        {
            get
            {
                return ".rar";
            }
        }

        public override IEnumerable<FileSystemInfo> ExtractFilesToTemp()
        {
            CheckFileInfoIsSet();
            CreateTempFolder();

            var tempEntries = new List<FileSystemInfo>();

            using (var stream = File.OpenRead(FileInfo.FullName))
            {
                using (var reader = RarReader.Open(stream))
                {
                    while (reader.MoveToNextEntry())
                    {
                        var entry = reader.Entry;
                        var tempPath = Path.Combine(TempFolder, entry.FilePath);

                        if (entry.IsDirectory)
                        {
                            Directory.CreateDirectory(tempPath);
                            tempEntries.Add(new DirectoryInfo(tempPath));
                        }
                        else
                        {
                            reader.WriteEntryToFile(tempPath);
                            tempEntries.Add(new FileInfo(tempPath));
                        }
                    }
                }
            }

            return tempEntries;
        }
    }
}
