namespace NIHEI.SC4Buddy.UserFolders.DataAccess
{
    using System.IO;
    using System.Reflection;
    using log4net;
    using Newtonsoft.Json;
    using NIHEI.SC4Buddy.Model;

    public class UserFolderDataAccess : IUserFolderDataAccess
    {
        public const string Filename = "UserFolder.json";

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UserFolder LoadUserFolder(string path)
        {
            Log.Info(string.Format("Loading user folder from path \"{0}\".", path));

            var file = new FileInfo(Path.Combine(path, Filename));

            if (!file.Exists)
            {
                throw new FileNotFoundException(string.Format("{0} does not exist.", file.FullName));
            }

            using (var reader = new StreamReader(file.FullName))
            {
                var json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<UserFolder>(json);
            }
        }

        public void SaveUserFolder(UserFolder userFolder)
        {
            Log.Info(string.Format("Saving user folder \"{0}\" (id: {1})", userFolder.Alias, userFolder.Id));

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
