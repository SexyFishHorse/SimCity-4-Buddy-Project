namespace NIHEI.SC4Buddy.Plugins.Control
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Asser.Sc4Buddy.Server.Api.V1.Client;
    using log4net;
    using Microsoft.VisualBasic.FileIO;
    using NIHEI.SC4Buddy.Configuration;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Plugins.DataAccess;
    using NIHEI.SC4Buddy.Remote;
    using NIHEI.SC4Buddy.Remote.Utils;
    using SearchOption = System.IO.SearchOption;

    public class PluginsController : IPluginsController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PluginsDataAccess pluginsDataAccess;

        private readonly IPluginMatcher pluginMatcher;

        private readonly IBuddyServerClient client;

        public PluginsController(PluginsDataAccess pluginsDataAccess, UserFolder userFolder, IPluginMatcher pluginMatcher, IBuddyServerClient client)
        {
            this.pluginsDataAccess = pluginsDataAccess;
            UserFolder = userFolder;
            this.pluginMatcher = pluginMatcher;
            this.client = client;

            ReloadPlugins();
        }

        public ICollection<Plugin> Plugins { get; set; }

        public UserFolder UserFolder { get; set; }

        public static bool IsDamnFile(UserFolder userFolder, string path)
        {
            var relativePath = path.Replace(userFolder.PluginFolderPath, string.Empty);

            return relativePath.StartsWith(@"\DAMN", StringComparison.OrdinalIgnoreCase)
                   && (relativePath.EndsWith("placeholder", StringComparison.OrdinalIgnoreCase)
                       || relativePath.EndsWith("DAMN-Indexer.cmd"));
        }

        public static bool IsBackgroundImage(string entity, UserFolder userFolder)
        {
            if (!userFolder.IsMainFolder)
            {
                return false;
            }

            var validFilenames = new[]
                                     {
                                         "Background3D0.png", "Background3D1.png", "Background3D2.png",
                                         "Background3D3.png", "Background3D4.png"
                                     };

            return validFilenames.Any(entity.EndsWith);
        }

        public void Add(Plugin plugin)
        {
            if (Plugins.Any(x => x.Id == plugin.Id))
            {
                throw new ArgumentException("A plugin with the id " + plugin.Id + " already exists.");
            }

            Plugins.Add(plugin);
            pluginsDataAccess.SavePlugins(Plugins, UserFolder);
        }

        public void Update(Plugin plugin)
        {
            if (Plugins.Any(x => x.Id == plugin.Id))
            {
                Plugins.Remove(plugin);
            }

            Plugins.Add(plugin);

            pluginsDataAccess.SavePlugins(Plugins, UserFolder);
        }

        public void Remove(Plugin plugin)
        {
            if (Plugins.All(x => x.Id != plugin.Id))
            {
                return;
            }

            Plugins.Remove(plugin);
            pluginsDataAccess.SavePlugins(Plugins, UserFolder);
        }

        public void UninstallPlugin(Plugin plugin)
        {
            var files = new PluginFile[plugin.PluginFiles.Count];
            plugin.PluginFiles.CopyTo(files, 0);
            foreach (var fileInfo in files.Select(file => new FileInfo(file.Path)))
            {
                if (fileInfo.Exists)
                {
                    FileSystem.DeleteFile(fileInfo.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

                    DeleteDirectoryIfEmpty(fileInfo.Directory);
                }
            }

            Remove(plugin);
        }

        public void QuarantineFiles(IEnumerable<PluginFile> files)
        {
            foreach (var file in files.Where(x => File.Exists(x.Path)))
            {
                var newPath = Path.Combine(Settings.Get(Settings.Keys.QuarantinedFiles), Path.GetRandomFileName());
                Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                File.Copy(file.Path, newPath);
                File.Delete(file.Path);

                file.QuarantinedFile = new QuarantinedFile { PluginFile = file, QuarantinedPath = newPath };
            }
        }

        public void UnquarantineFiles(IEnumerable<PluginFile> files)
        {
            foreach (var file in files.Where(x => x.QuarantinedFile != null && File.Exists(x.QuarantinedFile.QuarantinedPath)))
            {
                if (file.QuarantinedFile != null)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(file.Path));
                    File.Copy(file.QuarantinedFile.QuarantinedPath, file.Path);
                    File.Delete(file.QuarantinedFile.QuarantinedPath);
                }

                file.QuarantinedFile = null;
            }
        }

        public void RemoveFilesFromPlugins(ICollection<string> deletedFilePaths)
        {
            foreach (var deletedFilePath in deletedFilePaths)
            {
                foreach (var plugin in Plugins)
                {
                    var file = plugin.PluginFiles.FirstOrDefault(x => x.Path == deletedFilePath);

                    if (file != null)
                    {
                        plugin.PluginFiles.Remove(file);
                    }
                }
            }

            RemoveEmptyPlugins();
        }

        public void ReloadPlugins()
        {
            Plugins = pluginsDataAccess.LoadPlugins();
            UserFolder.Plugins = Plugins;
        }

        public int IdentifyNewPlugins()
        {
            ApiConnect.ThrowErrorOnConnectionOrDisabledFeature(Settings.Keys.DetectPlugins);

            var numIdentified = 0;

            foreach (var plugin in Plugins.Where(x => x.RemotePlugin == null))
            {
                var matchedPlugin = pluginMatcher.GetMostLikelyPluginForGroupOfFiles(plugin.PluginFiles);

                if (matchedPlugin == null)
                {
                    continue;
                }

                plugin.RemotePlugin = matchedPlugin;
                plugin.Name = matchedPlugin.Name;
                plugin.Author = matchedPlugin.Author;
                plugin.Link = matchedPlugin.Link;
                plugin.Description = matchedPlugin.Description;

                numIdentified++;
            }

            pluginsDataAccess.SavePlugins(Plugins, UserFolder);
            ReloadPlugins();

            return numIdentified;
        }

        public int UpdateKnownPlugins(BackgroundWorker backgroundWorker)
        {
            Log.Info("Update known plugins from the server");
            ApiConnect.ThrowErrorOnConnectionOrDisabledFeature(Settings.Keys.FetchInformationFromRemoteServer);
            Log.Debug("Feature enabled");

            var numUpdated = 0;
            var knownPlugins = Plugins.Where(x => x.RemotePlugin != null).ToList();
            var numProcessed = 0.0;
            foreach (var plugin in knownPlugins)
            {
                var remotePlugin = client.GetPlugin(plugin.RemotePluginId);

                if (remotePlugin == null)
                {
                    Log.Warn(
                        string.Format(
                            "The plugin \"{0}\" does not exist or has been removed from the server.",
                            plugin.RemotePluginId));
                }
                else
                {
                    Log.Debug(
                        string.Format(
                            "The plugin \"{0}\" ({1}) was updated as the remote plugin \"{2}\" ({3}).",
                            plugin.Name,
                            plugin.Id,
                            remotePlugin.Name,
                            remotePlugin.Id));
                    plugin.Name = remotePlugin.Name;
                    plugin.Link = remotePlugin.Link;
                    plugin.Author = remotePlugin.Author;
                    plugin.Description = remotePlugin.Description;

                    numUpdated++;
                }

                numProcessed++;

                backgroundWorker.ReportProgress(
                    (int)Math.Round((numProcessed / knownPlugins.Count) * 100),
                    string.Format("Processed {0} of {1} known plugins.", numProcessed, knownPlugins.Count));
            }

            Log.Info(string.Format("Updated {0} plugins.", numUpdated));

            return numUpdated;
        }

        public int NumberOfRecognizedPlugins(UserFolder userFolder)
        {
            return Plugins.Count(x => x.RemotePlugin != null);
        }

        public int RemoveEmptyPlugins()
        {
            var emptyPlugins = Plugins.Where(x => !x.PluginFiles.Any()).ToList();

            var counter = emptyPlugins.Count();

            foreach (var emptyPlugin in emptyPlugins)
            {
                Remove(emptyPlugin);
            }

            return counter;
        }

        private void DeleteDirectoryIfEmpty(DirectoryInfo directory)
        {
            while (true)
            {
                if (!directory.EnumerateFileSystemInfos("*", SearchOption.AllDirectories).Any())
                {
                    directory.Delete();

                    if (directory.Parent != null)
                    {
                        directory = directory.Parent;
                        continue;
                    }
                }

                break;
            }
        }
    }
}
