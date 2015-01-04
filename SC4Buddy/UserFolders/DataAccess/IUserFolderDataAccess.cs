namespace NIHEI.SC4Buddy.UserFolders.DataAccess
{
    using NIHEI.SC4Buddy.Model;

    public interface IUserFolderDataAccess
    {
        UserFolder LoadUserFolder(string path);

        void SaveUserFolder(UserFolder userFolder);
    }
}