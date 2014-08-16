namespace NIHEI.SC4Buddy.Control
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualBasic.FileIO;
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

        public IEnumerable<FileTypeInfo> FileTypes { get; set; }

        public void LoadFileTypesFromDisc()
        {
            var fileLocation = Path.Combine(StorageLocation, "NonPluginFileTypes.json");

            var newFileTypes = new Collection<FileTypeInfo>();

            if (File.Exists(fileLocation))
            {
                using (var reader = new StreamReader(fileLocation))
                {
                    var json = reader.ReadToEnd();

                    dynamic fileTypeJson = JArray.Parse(json);

                    foreach (var fileType in fileTypeJson)
                    {
                        newFileTypes.Add(
                            new FileTypeInfo
                            {
                                Extension = fileType.Extension,
                                DescriptiveName = fileType.DescriptiveName,
                                Description = fileType.Description
                            });
                    }
                }
            }

            FileTypes = newFileTypes;
        }

        public ICollection<NonPluginFileTypeCandidateInfo> GetFilesAndFoldersToRemove(UserFolder userFolder)
        {
            var fileTypeCandidateInfos = GetCandiateFileTypeInfos(userFolder);

            var emptyFolders = GetEmptyFolders(userFolder);

            var output = new Collection<NonPluginFileTypeCandidateInfo>();

            foreach (var fileTypeCandidateInfo in fileTypeCandidateInfos)
            {
                output.Add(fileTypeCandidateInfo);
            }

            if (emptyFolders.Any())
            {
                output.Add(
                    new NonPluginFileTypeCandidateInfo
                    {
                        FileTypeInfo =
                            new FileTypeInfo
                            {
                                Extension = string.Empty,
                                Description = "Empty Folders",
                                DescriptiveName = "Folders"
                            },
                        NumberOfEntities = emptyFolders.Count
                    });
            }

            return output;
        }

        public int RemoveNonPluginFiles(UserFolder userFolder)
        {
            var filesToDelete = GetCandidateFiles(userFolder);

            foreach (var file in filesToDelete)
            {
                FileSystem.DeleteFile(file, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }

            var foldersToDelete = GetEmptyFolders(userFolder);

            foreach (var folder in foldersToDelete.Where(Directory.Exists))
            {
                FileSystem.DeleteDirectory(folder, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }

            return foldersToDelete.Count();
        }

        private static IList<string> GetEmptyFolders(UserFolder userFolder)
        {
            var folders = Directory.EnumerateDirectories(userFolder.PluginFolderPath, "*", SearchOption.AllDirectories).ToList();
            var foldersToDelete =
                folders.Where(x => !new DirectoryInfo(x).EnumerateFiles("*", SearchOption.AllDirectories).Any()).ToList();
            return foldersToDelete;
        }

        private IEnumerable<string> GetCandidateFiles(UserFolder userFolder)
        {
            var files = Directory.EnumerateFiles(userFolder.PluginFolderPath, "*", SearchOption.AllDirectories).ToList();
            var filesToDelete = new List<string>();

            foreach (var fileType in FileTypes)
            {
                filesToDelete.AddRange(files.Where(x => x.ToUpperInvariant().EndsWith(fileType.Extension.ToUpperInvariant())));
            }

            return filesToDelete;
        }

        private IEnumerable<NonPluginFileTypeCandidateInfo> GetCandiateFileTypeInfos(UserFolder userFolder)
        {
            var files = Directory.EnumerateFiles(userFolder.PluginFolderPath, "*", SearchOption.AllDirectories).ToList();
            var output = new Collection<NonPluginFileTypeCandidateInfo>();

            foreach (var fileTypeInfo in FileTypes)
            {
                var numberOfFiles =
                    files.Count(x => x.EndsWith(fileTypeInfo.Extension, StringComparison.OrdinalIgnoreCase));

                if (numberOfFiles > 0)
                {
                    output.Add(new NonPluginFileTypeCandidateInfo { FileTypeInfo = fileTypeInfo, NumberOfEntities = numberOfFiles });
                }
            }

            return output;
        }
    }
}
