namespace NIHEI.SC4Buddy.Plugins.Installer.InstallerEventArgs
{
    using System.Collections.Generic;
    using System.IO;

    public class ReadmeFilesEventArgs : InstallPluginEventArgs
    {
        public ReadmeFilesEventArgs(FileInfo fileInfo, IEnumerable<FileInfo> readmeFiles)
            : base(fileInfo)
        {
            ReadmeFiles = readmeFiles;
        }

        public IEnumerable<FileInfo> ReadmeFiles { get; private set; }
    }
}