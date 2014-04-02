namespace NIHEI.SC4Buddy.Installer.InstallerEventArgs
{
    using System.IO;

    using NIHEI.SC4Buddy.Model;

    public class InstalledPluginEventArgs : InstallPluginEventArgs
    {
        public InstalledPluginEventArgs(FileInfo fileInfo, Plugin plugin)
            : base(fileInfo)
        {
            Plugin = plugin;
        }

        public Plugin Plugin { get; private set; }
    }
}