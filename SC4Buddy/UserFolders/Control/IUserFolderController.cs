namespace NIHEI.SC4Buddy.UserFolders.Control
{
    using System.Collections.Generic;
    using NIHEI.SC4Buddy.Model;

    public interface IUserFolderController
    {
        IEnumerable<UserFolder> UserFolders { get; }

        void Update(UserFolder userFolder);

        void SaveChanges();
    }
}
