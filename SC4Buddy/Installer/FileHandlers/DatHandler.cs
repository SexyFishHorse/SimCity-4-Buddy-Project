namespace NIHEI.SC4Buddy.Installer.FileHandlers
{
    using System.Collections.Generic;
    using System.IO;

    public class DatHandler : BaseHandler
    {
        public override string RequiredExtension
        {
            get
            {
                return ".dat";
            }
        }

        public override IEnumerable<FileSystemInfo> ExtractFilesToTemp()
        {
            CheckFileInfoIsSet();
            CreateTempFolder();

            var newPath = Path.Combine(TempFolder, FileInfo.Name);
            File.Copy(FileInfo.FullName, newPath);
            return new List<FileSystemInfo> { new FileInfo(newPath) };
        }
    }
}
