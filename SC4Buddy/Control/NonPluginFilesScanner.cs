namespace NIHEI.SC4Buddy.Control
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualBasic.FileIO;

    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.Installer.FileHandlers;
    using Newtonsoft.Json.Linq;
    using NIHEI.SC4Buddy.Model;
    using SearchOption = System.IO.SearchOption;

    public class NonPluginFilesScanner
    {
        public NonPluginFilesScanner(string storageLocation)
        {
            StorageLocation = storageLocation;

            LoadFileTypesFromDisc();
        }

        public string StorageLocation { get; set; }

        public IEnumerable<string> FileTypes { get; set; }

        public void LoadFileTypesFromDisc()
        {
            var fileLocation = Path.Combine(StorageLocation, "NonPluginFileTypes.json");

            var newFileTypes = new Collection<string>();

            if (File.Exists(fileLocation))
            {
                using (var reader = new StreamReader(fileLocation))
                {
                    var json = reader.ReadToEnd();

                    dynamic fileTypeJson = JArray.Parse(json);

                    foreach (string fileType in fileTypeJson)
                    {
                        newFileTypes.Add(fileType);
                    }
                }
            }

            FileTypes = newFileTypes;
        }
        public bool HasFilesAndFoldersToRemove(UserFolder userFolder, out int numFiles, out int numFolders)
        {
            var filesToDelete = GetFilesToDelete(userFolder);
            numFiles = filesToDelete.Count();

            var foldersToDelete = GetFoldersToDelete(userFolder);
            numFolders = foldersToDelete.Count();

            return numFiles > 0 || numFolders > 0;
        }

        public int RemoveNonPluginFiles(UserFolder userFolder)
        {
            var filesToDelete = GetFilesToDelete(userFolder);

            foreach (var file in filesToDelete)
            {
                FileSystem.DeleteFile(file, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }

            var foldersToDelete = GetFoldersToDelete(userFolder);

            foreach (var folder in foldersToDelete.Where(Directory.Exists))
            {
                FileSystem.DeleteDirectory(folder, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }

            return foldersToDelete.Count();
        }

        private static IList<string> GetFoldersToDelete(UserFolder userFolder)
        {
            var folders = Directory.EnumerateDirectories(userFolder.PluginFolderPath, "*", SearchOption.AllDirectories).ToList();
            var foldersToDelete =
                folders.Where(x => !new DirectoryInfo(x).EnumerateFiles("*", SearchOption.AllDirectories).Any()).ToList();
            return foldersToDelete;
        }

        private static IEnumerable<string> GetFilesToDelete(UserFolder userFolder)
        {
            var files = Directory.EnumerateFiles(userFolder.PluginFolderPath, "*", SearchOption.AllDirectories);
            var filesToDelete =
                files.Where(
                    x =>
                    !BaseHandler.IsPluginFile(x) && !UserFolderController.IsBackgroundImage(x, userFolder)
                    && !UserFolderController.IsDamnFile(userFolder, x)).ToList();
            return filesToDelete;
        }
    }
}
