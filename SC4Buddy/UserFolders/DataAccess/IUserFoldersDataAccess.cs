namespace Nihei.SC4Buddy.UserFolders.DataAccess
{
    using System.Collections.Generic;
    using Nihei.SC4Buddy.Model;

    public interface IUserFoldersDataAccess
    {
        ICollection<UserFolder> LoadUserFolders();

        void SaveUserFolders(IEnumerable<UserFolder> userFolders);
    }
}
