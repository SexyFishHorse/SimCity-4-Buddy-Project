namespace NIHEI.SC4Buddy.UserFolders.DataAccess
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NIHEI.SC4Buddy.Application.Utilities;
    using NIHEI.SC4Buddy.Model;

    public class UserFoldersDataAccess : IUserFoldersDataAccess
    {
        public const string Filename = "UserFolders.json";

        public ICollection<UserFolder> LoadUserFolders()
        {
            var path = Path.Combine(FileSystemLocationsUtil.LocalApplicationDataDirectory, Filename);
            var fileInfo = new FileInfo(path);

            var userFolders = new Collection<UserFolder>();

            if (!fileInfo.Exists)
            {
                return userFolders;
            }

            using (var reader = new StreamReader(path))
            {
                var json = reader.ReadToEnd();
                dynamic userFoldersJson = JArray.Parse(json);

                foreach (var userFolderJson in userFoldersJson)
                {
                    var userFolder = new UserFolder
                    {
                        Id = userFolderJson.Id,
                        Alias = userFolderJson.Alias,
                        FolderPath = userFolderJson.FolderPath,
                        IsMainFolder = userFolderJson.IsMainFolder,
                        IsStartupFolder = userFolderJson.IsStartupFolder
                    };

                    userFolders.Add(userFolder);
                }

                return userFolders;
            }
        }

        public void SaveUserFolders(IEnumerable<UserFolder> userFolders)
        {
            var fileInfo = new FileInfo(Path.Combine(FileSystemLocationsUtil.LocalApplicationDataDirectory, Filename));

            if (fileInfo.DirectoryName == null)
            {
                throw new DirectoryNotFoundException(string.Format("The location string {0} does not contain a directory name.", fileInfo.FullName));
            }

            Directory.CreateDirectory(fileInfo.DirectoryName);

            using (var writer = new StreamWriter(fileInfo.FullName))
            {
                var json = JsonConvert.SerializeObject(userFolders);
                writer.Write(json);
            }
        }
    }
}
