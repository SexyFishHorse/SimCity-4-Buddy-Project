namespace Nihei.SC4Buddy.Plugins.Control
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using Nihei.Common.IO;
    using Nihei.SC4Buddy.Model;
    using Nihei.SC4Buddy.Plugins.Services;
    using Nihei.SC4Buddy.UserFolders.Control;

    public class FolderScannerController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event EventHandler NewFilesFound;

        public List<string> NewFiles { get; private set; }

        public bool ScanFolder(UserFolder userFolder)
        {
            try
            {
                var folderScanner = new FolderScanner(userFolder);

                if (!folderScanner.ScanFolderForNewFiles())
                {
                    return false;
                }

                NewFiles = folderScanner.NewFiles.ToList();

                NewFilesFound(this, EventArgs.Empty);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Error during folder scan: {0}", ex));

                return false;
            }
        }

        public int AutoGroupKnownFiles(UserFolder userFolder, IPluginsController pluginController, IPluginMatcher pluginMatcher, BackgroundWorker backgroundWorker)
        {
            var files = new Collection<PluginFile>();
            var numFiles = NewFiles.Count;
            var filesProcessed = 0.0;

            Log.Info("Converting new files to plugin objects");
            foreach (var file in NewFiles)
            {
                files.Add(new PluginFile { Path = file, Checksum = Md5ChecksumUtility.CalculateChecksum(file).ToHex() });
            }

            Log.Info("Reloading plugin data from the server");
            pluginMatcher.ReloadData();

            Log.Info("Matching files");
            var filesAndPlugins = pluginMatcher.GetMostLikelyPluginForEachFile(files, backgroundWorker);

            var plugins = new Collection<Plugin>();

            Log.Info("Merging results.");
            foreach (var fileAndPlugin in filesAndPlugins)
            {
                if (backgroundWorker.CancellationPending)
                {
                    return 0;
                }

                var plugin = new Plugin { Id = fileAndPlugin.Value.Id };
                if (!plugins.Contains(plugin))
                {
                    plugin.Name = fileAndPlugin.Value.Name;
                    plugin.Author = fileAndPlugin.Value.Author;
                    plugin.Link = fileAndPlugin.Value.Link;
                    plugin.Description = fileAndPlugin.Value.Description;
                    plugin.PluginFiles.Add(fileAndPlugin.Key);
                    plugins.Add(plugin);
                }
                else
                {
                    plugins.First(x => x.Id == fileAndPlugin.Value.Id).PluginFiles.Add(fileAndPlugin.Key);
                }

                NewFiles.Remove(fileAndPlugin.Key.Path);
                filesProcessed++;

                backgroundWorker.ReportProgress(
                    ((int)Math.Floor(filesProcessed / numFiles) * 5) + 95,
                    string.Format("Updated {0} of {1} files.", filesProcessed, numFiles));
            }

            Log.Info("Saving found plugins.");
            foreach (var plugin in plugins)
            {
                pluginController.Add(plugin);
            }

            pluginController.ReloadPlugins();

            Log.Info("Done with auto grouping plugins.");
            return plugins.Count;
        }
    }
}
