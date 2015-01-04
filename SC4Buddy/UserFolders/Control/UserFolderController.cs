namespace NIHEI.SC4Buddy.UserFolders.Control
{
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.UserFolders.DataAccess;

    public class UserFolderController : IUserFolderController
    {
        private readonly IUserFolderDataAccess userFolderDataAccess;

        public UserFolderController(IUserFolderDataAccess userFolderDataAccess)
        {
            this.userFolderDataAccess = userFolderDataAccess;
        }

        public void Update(UserFolder userFolder)
        {
            userFolderDataAccess.SaveUserFolder(userFolder);
        }
    }
}
