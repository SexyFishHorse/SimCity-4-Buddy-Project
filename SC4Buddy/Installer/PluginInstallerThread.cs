namespace NIHEI.SC4Buddy.Installer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using log4net;

    using NIHEI.Common.IO;
    using NIHEI.SC4Buddy.Control.Plugins;
    using NIHEI.SC4Buddy.Installer.FileHandlers;
    using NIHEI.SC4Buddy.Installer.InstallerEventArgs;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Properties;
    using NIHEI.SC4Buddy.View.Plugins;

    using SharpCompress.Common;

    public class PluginInstallerThread
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PluginFileController pluginFileController;

        private readonly PluginController pluginController;

        public PluginInstallerThread(PluginController pluginController, PluginFileController pluginFileController)
        {
            this.pluginFileController = pluginFileController;
            this.pluginController = pluginController;
        }

        public delegate void InstallPluginEventHandler(PluginInstallerThread sender, InstallPluginEventArgs args);

        public delegate void InstallPluginFailureEventHandler(PluginInstallerThread sender, InstallPluginFailureEventArgs args);

        public delegate void InstallProgressChangedEventHandler(PluginInstallerThread sender, InstallProgressEventArgs args);

        public delegate void InstalledPluginEventHandler(PluginInstallerThread sender, InstalledPluginEventArgs args);

        public delegate void ReadmeFilesFoundEventHandler(PluginInstallerThread sender, ReadmeFilesEventArgs args);

        public delegate void NotPartOneOfMultipartDetectedEventHandler(
            PluginInstallerThread sender, InstallPluginEventArgs args);

        public event InstallPluginEventHandler InstallingPlugin;

        public event InstalledPluginEventHandler PluginInstalled;

        public event InstallPluginFailureEventHandler PluginInstallFailed;

        public event InstallProgressChangedEventHandler InstallProgressChanged;

        public event EventHandler AllPluginsInstalled;

        public event ReadmeFilesFoundEventHandler ReadmeFilesFound;

        public event NotPartOneOfMultipartDetectedEventHandler NotPartOneOfMultipartDetected;

        public string[] FilesToInstall { get; set; }

        public InstallPluginsForm Form { get; set; }

        public UserFolder UserFolder { get; set; }

        public void Install()
        {
            var installer = new PluginInstaller();

            foreach (var file in FilesToInstall)
            {
                var fileInfo = new FileInfo(file);
                RaiseInstallPluginEvent(fileInfo);
                Log.Info("Installing plugin " + fileInfo.Name);

                try
                {
                    var installedFiles = new List<PluginFile>();

                    var randomFileName = Path.GetRandomFileName();
                    randomFileName = randomFileName.Substring(0, randomFileName.Length - 4) + DateTime.UtcNow.Ticks;

                    try
                    {
                        installer.ExtractToTempFolder(
                            fileInfo, Path.Combine(Path.GetTempPath(), "SC4Buddy", randomFileName));
                    }
                    catch (MultiVolumeExtractionException)
                    {
                        RaiseNotPartOneOfMultipartDetectedEvent(fileInfo);
                        continue;
                    }

                    RaiseInstallProgressEvent(fileInfo, 25, LocalizationStrings.FilesExtractedToTemporaryFolder);

                    HandleReadmeFiles(fileInfo, installer);

                    installedFiles.AddRange(HandleExecutableFiles(installer, UserFolder));

                    installedFiles.AddRange(HandlePluginFiles(installer));

                    if (!installedFiles.Any())
                    {
                        var validExtensions = string.Format("({0})", string.Join(", ", BaseHandler.PluginFileExtensions));
                        var errorMessage =
                            string.Format(
                                LocalizationStrings.ThePluginDigNotContainAnyValidFilesToInstall,
                                validExtensions);
                        RaisePluginInstallFailedEvent(fileInfo, errorMessage);
                        continue;
                    }

                    RaiseInstallProgressEvent(fileInfo, 75, LocalizationStrings.FilesMovedToUserFolder);

                    var plugin = new Plugin { Name = new FileInfo(file).Name, UserFolder = UserFolder };

                    SavePluginInformation(plugin, installedFiles);

                    RaisePluginInstalledEvent(fileInfo, plugin);

                    Log.Info("Installation successfull.");
                }
                catch (Exception ex)
                {
                    var errorMessage = string.Format(
                        "Unexpected exception during plugin install: [{0}] {1}",
                        new object[] { ex.GetType().Name, ex.Message });

                    Log.Error("Installation failed", ex);

                    RaisePluginInstallFailedEvent(fileInfo, errorMessage);
                }
            }

            RaiseAllPluginsInstalledEvent();
        }

        protected virtual void RaiseNotPartOneOfMultipartDetectedEvent(FileInfo fileInfo)
        {
            NotPartOneOfMultipartDetected.Invoke(this, new InstallPluginEventArgs(fileInfo));
        }

        protected virtual void RaiseInstallProgressEvent(FileInfo fileInfo, int progress, string message)
        {
            InstallProgressChanged.Invoke(this, new InstallProgressEventArgs(fileInfo, progress, message));
        }

        protected virtual void RaisePluginInstalledEvent(FileInfo fileInfo, Plugin plugin)
        {
            PluginInstalled.Invoke(this, new InstalledPluginEventArgs(fileInfo, plugin));
        }

        protected virtual void RaiseAllPluginsInstalledEvent()
        {
            AllPluginsInstalled.Invoke(this, EventArgs.Empty);
        }

        protected virtual void RaisePluginInstallFailedEvent(FileInfo fileInfo, string errorMessage)
        {
            PluginInstallFailed.Invoke(this, new InstallPluginFailureEventArgs(fileInfo, errorMessage));
        }

        protected virtual void RaiseInstallPluginEvent(FileInfo fileInfo)
        {
            InstallingPlugin.Invoke(this, new InstallPluginEventArgs(fileInfo));
        }

        protected virtual void RaiseReadmeFilesFoundEvent(FileInfo fileInfo, IEnumerable<FileInfo> readmeFiles)
        {
            ReadmeFilesFound.Invoke(this, new ReadmeFilesEventArgs(fileInfo, readmeFiles));
        }

        private void SavePluginInformation(Plugin plugin, IEnumerable<PluginFile> installedFiles)
        {
            foreach (var installedFile in installedFiles.Distinct(new PluginFileComparer()))
            {
                installedFile.Plugin = plugin;
                plugin.PluginFiles.Add(installedFile);

                var existingPlugin =
                    pluginFileController.Files.FirstOrDefault(
                        x => x.Path.Equals(installedFile.Path, StringComparison.OrdinalIgnoreCase));
                if (existingPlugin != null)
                {
                    pluginFileController.Delete(existingPlugin, false);
                }
            }

            var numDeleted = pluginController.RemoveEmptyPlugins();

            if (numDeleted <= 0)
            {
                pluginController.Add(plugin, false);
            }

            pluginController.SaveChanges();
        }

        private IEnumerable<PluginFile> HandlePluginFiles(PluginInstaller installer)
        {
            if (installer.PluginFiles.Any())
            {
                installer.MoveToUserFolder(UserFolder);
                return installer.InstalledFiles;
            }

            return new List<PluginFile>(0);
        }

        private IEnumerable<PluginFile> HandleExecutableFiles(PluginInstaller installer, UserFolder userFolder)
        {
            return installer.Executables.Any() ? RunExecutables(installer, userFolder) : new List<PluginFile>(0);
        }

        private void HandleReadmeFiles(FileInfo fileInfo, PluginInstaller installer)
        {
            if (installer.ReadmeFiles.Any())
            {
                RaiseReadmeFilesFoundEvent(fileInfo, installer.ReadmeFiles);
            }
        }

        private IEnumerable<PluginFile> RunExecutables(PluginInstaller installer, UserFolder userFolder)
        {
            var installedFiles = new List<PluginFile>();

            foreach (
                var executable in
                    installer.Executables.Where(
                        x => Settings.Default.InstallerAutoRunExecutables || Form.AskToRunExecutable(x)))
            {
                using (var folderListener = new UserFolderListener(userFolder))
                {
                    folderListener.Start();

                    var process = Process.Start(executable.FullName);

                    if (process != null)
                    {
                        process.WaitForExit();
                    }
                    else
                    {
                        Form.ShowInstallationDidNotStartDialog();
                    }

                    installedFiles.AddRange(HandleListenerFileChanges(folderListener));
                }
            }

            return installedFiles;
        }

        private IEnumerable<PluginFile> HandleListenerFileChanges(UserFolderListener folderListener)
        {
            var installedFiles = new List<PluginFile>();
            pluginFileController.DeleteFilesByPath(folderListener.DeletedFiles);

            installedFiles.AddRange(
                folderListener.CreatedFiles.Where(File.Exists).Select(
                    file => new PluginFile { Path = file, Checksum = Md5ChecksumUtility.CalculateChecksum(file).ToHex() }));

            pluginFileController.DeleteFilesByPath(folderListener.ChangedFiles);

            installedFiles.AddRange(
                folderListener.ChangedFiles.Where(File.Exists).Select(
                    file => new PluginFile { Path = file, Checksum = Md5ChecksumUtility.CalculateChecksum(file).ToHex() }));

            var oldPaths = new List<string>();
            var newPaths = new List<string>();
            foreach (var file in folderListener.RenamedFiles)
            {
                oldPaths.Add(file.Key);
                newPaths.Add(file.Value);
            }

            pluginFileController.DeleteFilesByPath(oldPaths);

            installedFiles.AddRange(
                newPaths.Where(File.Exists).Select(
                    file => new PluginFile { Path = file, Checksum = Md5ChecksumUtility.CalculateChecksum(file).ToHex() }));

            return installedFiles;
        }
    }
}
