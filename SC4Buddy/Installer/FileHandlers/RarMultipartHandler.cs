namespace NIHEI.SC4Buddy.Installer.FileHandlers
{
    using System.Collections.Generic;
    using System.IO;

    public class RarMultipartHandler : BaseHandler
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
            throw new System.NotImplementedException();
        }
    }
}
