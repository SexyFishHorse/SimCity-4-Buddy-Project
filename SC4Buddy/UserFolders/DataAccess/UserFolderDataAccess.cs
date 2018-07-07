namespace Nihei.SC4Buddy.UserFolders.DataAccess
{
    using System.IO;
    using System.Reflection;
    using log4net;
    using Newtonsoft.Json;
    using Nihei.SC4Buddy.Model;
    using Nihei.SC4Buddy.Utils;

    public class UserFolderDataAccess : IUserFolderDataAccess
    {
        public const string Filename = "UserFolder.json";

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IJsonFileWriter writer;

        public UserFolderDataAccess(IJsonFileWriter writer)
        {
            this.writer = writer;
        }

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

            writer.WriteToFile(fileInfo, userFolder);
        }
    }
}
