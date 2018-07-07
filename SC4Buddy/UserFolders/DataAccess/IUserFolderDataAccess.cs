namespace Nihei.SC4Buddy.UserFolders.DataAccess
{
    using Nihei.SC4Buddy.Model;

    public interface IUserFolderDataAccess
    {
        UserFolder LoadUserFolder(string path);

        void SaveUserFolder(UserFolder userFolder);
    }
}