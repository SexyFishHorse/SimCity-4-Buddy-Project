namespace NIHEI.SC4Buddy.Installer
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using NIHEI.SC4Buddy.Entities;

    public class UserFolderListener : IDisposable
    {
        public UserFolderListener(UserFolder userFolder)
        {
            UserFolder = userFolder;
            ChangedFiles = new List<string>();
            CreatedFiles = new List<string>();
            DeletedFiles = new List<string>();
            RenamedFiles = new Dictionary<string, string>();

            Watcher = new FileSystemWatcher(UserFolder.Path, "*")
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
            };
            Watcher.Changed += WatcherEventListener;
            Watcher.Created += WatcherEventListener;
            Watcher.Deleted += WatcherEventListener;
            Watcher.Renamed += WatcherEventListener;
        }

        public UserFolder UserFolder { get; private set; }

        public ICollection<string> ChangedFiles { get; set; }

        public ICollection<string> CreatedFiles { get; set; }

        public ICollection<string> DeletedFiles { get; set; }

        public IDictionary<string, string> RenamedFiles { get; set; }

        private FileSystemWatcher Watcher { get; set; }

        public void Start()
        {
            Watcher.EnableRaisingEvents = true;
        }

        public void Dispose()
        {
            Watcher.Dispose();
        }

        private void WatcherEventListener(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            switch (fileSystemEventArgs.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    ChangedFiles.Add(fileSystemEventArgs.FullPath);
                    break;
                case WatcherChangeTypes.Created:
                    CreatedFiles.Add(fileSystemEventArgs.FullPath);
                    break;
                case WatcherChangeTypes.Deleted:
                    DeletedFiles.Add(fileSystemEventArgs.FullPath);
                    break;
                case WatcherChangeTypes.Renamed:
                    {
                        var renamedEventArgs = (RenamedEventArgs)fileSystemEventArgs;
                        RenamedFiles.Add(renamedEventArgs.OldFullPath, renamedEventArgs.FullPath);
                    }

                    break;
            }
        }
    }
}
