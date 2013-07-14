namespace NIHEI.SC4Buddy.Installer.InstallerEventArgs
{
    using System.IO;

    public class InstallProgressEventArgs : InstallPluginEventArgs
    {
        public InstallProgressEventArgs(FileInfo fileInfo, int progress, string message)
            : base(fileInfo)
        {
            Progress = progress;
            Message = message;
        }

        public int Progress { get; private set; }

        public string Message { get; private set; }
    }
}