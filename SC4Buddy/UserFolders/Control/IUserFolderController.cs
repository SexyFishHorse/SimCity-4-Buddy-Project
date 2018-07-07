namespace Nihei.SC4Buddy.UserFolders.Control
{
    using Nihei.SC4Buddy.Model;

    public interface IUserFolderController
    {
        void Update(UserFolder userFolder);

        UserFolder LoadUserFolder(string path);
    }
}
