namespace NIHEI.SC4Buddy.Installer.InstallerEventArgs
{
    using System;
    using System.IO;

    public class InstallPluginEventArgs : EventArgs
    {
        public InstallPluginEventArgs(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }

        public FileInfo FileInfo { get; private set; }
    }
}