namespace Nihei.SC4Buddy.Plugins.Control
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using Microsoft.VisualBasic.FileIO;
    using Newtonsoft.Json.Linq;
    using Nihei.SC4Buddy.Model;
    using SearchOption = System.IO.SearchOption;

    public class NonPluginFilesScanner
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

        public NonPluginFileRemovalSummary RemoveNonPluginFiles(UserFolder userFolder, IEnumerable<FileTypeInfo> fileTypesToRemove)
        {
            var filesToDelete = GetFilesToDelete(userFolder, fileTypesToRemove);

            var numFiles = 0;
            var numFolders = 0;

            var errors = new Dictionary<string, Exception>();

            foreach (var file in filesToDelete)
            {
                try
                {
                    FileSystem.DeleteFile(file, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    numFiles++;
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format("Unable to delete file \"{0}\", Error: {1}", file, ex.Message));
                    errors.Add(file, ex);
                }
            }

            var foldersToDelete = GetEmptyFolders(userFolder);

            foreach (var folder in foldersToDelete.Where(Directory.Exists))
            {
                try
                {
                    FileSystem.DeleteDirectory(folder, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    numFolders++;
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format("Unable to delete the folder \"{0}\", Error: {1}", folder, ex.Message));
                    errors.Add(folder, ex);
                }
            }

            return new NonPluginFileRemovalSummary
            {
                NumFilesRemoved = numFiles,
                NumFoldersRemoved = numFolders,
                Errors = errors
            };
        }

        private static IList<string> GetEmptyFolders(UserFolder userFolder)
        {
            var folders = Directory.EnumerateDirectories(userFolder.PluginFolderPath, "*", SearchOption.AllDirectories).ToList();
            var foldersToDelete =
                folders.Where(x => !new DirectoryInfo(x).EnumerateFiles("*", SearchOption.AllDirectories).Any()).ToList();
            return foldersToDelete;
        }

        private IEnumerable<string> GetFilesToDelete(UserFolder userFolder, IEnumerable<FileTypeInfo> fileTypesToRemove)
        {
            var files = Directory.EnumerateFiles(userFolder.PluginFolderPath, "*", SearchOption.AllDirectories).ToList();
            var filesToDelete = new List<string>();

            foreach (var fileType in fileTypesToRemove)
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
