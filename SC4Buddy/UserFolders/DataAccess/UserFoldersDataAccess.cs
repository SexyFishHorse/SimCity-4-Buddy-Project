namespace Nihei.SC4Buddy.UserFolders.DataAccess
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Reflection;
    using log4net;
    using Newtonsoft.Json.Linq;
    using Nihei.SC4Buddy.Application.Utilities;
    using Nihei.SC4Buddy.Model;
    using Nihei.SC4Buddy.Utils;

    public class UserFoldersDataAccess : IUserFoldersDataAccess
    {
        public const string Filename = "UserFolders.json";

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IJsonFileWriter writer;

        public UserFoldersDataAccess(IJsonFileWriter writer)
        {
            this.writer = writer;
        }

        public ICollection<UserFolder> LoadUserFolders()
        {
            Log.Info("Loading all user folders.");

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
            Log.Info("Save all user folders.");
            var fileInfo = new FileInfo(Path.Combine(FileSystemLocationsUtil.LocalApplicationDataDirectory, Filename));

            writer.WriteToFile(fileInfo, userFolders);
        }
    }
}
