namespace NIHEI.SC4Buddy.Installer.InstallerEventArgs
{
    using System.IO;

    public class InstallPluginFailureEventArgs : InstallPluginEventArgs
    {
        public InstallPluginFailureEventArgs(FileInfo fileInfo, string errorMessage)
            : base(fileInfo)
        {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; private set; }
    }
}