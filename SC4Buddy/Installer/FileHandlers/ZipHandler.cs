namespace NIHEI.SC4Buddy.Installer.FileHandlers
{
    using System.Collections.Generic;
    using System.IO;

    using SharpCompress.Reader;

    public class ZipHandler : BaseHandler
    {
        public override string RequiredExtension
        {
            get
            {
                return ".zip";
            }
        }

        public override IEnumerable<FileSystemInfo> ExtractFilesToTemp()
        {
            CheckFileInfoIsSet();
            CreateTempFolder();

            var tempEntries = new List<FileSystemInfo>();

            using (var stream = File.OpenRead(FileInfo.FullName))
            {
                using (var reader = ReaderFactory.Open(stream))
                {
                    while (reader.MoveToNextEntry())
                    {
                        var entry = reader.Entry;
                        var tempPath = Path.Combine(TempFolder, entry.FilePath).Replace("/", @"\");

                        if (entry.IsDirectory)
                        {
                            Directory.CreateDirectory(tempPath);
                            tempEntries.Add(new DirectoryInfo(tempPath));
                        }
                        else
                        {
                            Directory.CreateDirectory(tempPath.Substring(0, tempPath.LastIndexOf(@"\", System.StringComparison.Ordinal)));
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
