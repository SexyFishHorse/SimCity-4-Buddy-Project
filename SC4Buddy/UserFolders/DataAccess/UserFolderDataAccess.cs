namespace NIHEI.SC4Buddy.UserFolders.DataAccess
{
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NIHEI.SC4Buddy.Model;

    public class UserFolderDataAccess : IUserFolderDataAccess
    {
        public const string Filename = "UserFolder.json";

        public UserFolder LoadUserFolder(string path)
        {
            var file = new FileInfo(Path.Combine(path, Filename));

            if (!file.Exists)
            {
                throw new FileNotFoundException(string.Format("{0} does not exist.", file.FullName));
            }

            using (var reader = new StreamReader(file.FullName))
            {
                var json = reader.ReadToEnd();
                dynamic userFolderJson = JArray.Parse(json);

                return new UserFolder
                {
                    Id = userFolderJson.Id,
                    FolderPath = file.DirectoryName,
                    Alias = userFolderJson.Alias,
                    IsMainFolder = userFolderJson.IsMainFolder,
                    IsStartupFolder = userFolderJson.IsStartupFolder
                };
            }
        }

        public void SaveUserFolder(UserFolder userFolder)
        {
            var fileInfo = new FileInfo(Path.Combine(userFolder.FolderPath, Filename));

            if (fileInfo.DirectoryName == null)
            {
                throw new DirectoryNotFoundException(string.Format("The location string {0} does not contain a directory name.", fileInfo.FullName));
            }

            Directory.CreateDirectory(fileInfo.DirectoryName);

            using (var writer = new StreamWriter(fileInfo.FullName))
            {
                var json = JsonConvert.SerializeObject(userFolder);
                writer.Write(json);
            }
        }
    }
}
