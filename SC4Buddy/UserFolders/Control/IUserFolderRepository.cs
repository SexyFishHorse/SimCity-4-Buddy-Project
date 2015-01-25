namespace NIHEI.SC4Buddy.UserFolders.Control
{
    using System;
    using System.Collections.Generic;
    using NIHEI.SC4Buddy.Model;

    public interface IUserFolderRepository
    {
        IEnumerable<UserFolder> UserFolders { get; }

        bool ValidatePath(string path, Guid currentId);

        bool ValidateAlias(string alias, Guid currentId);

        void Delete(UserFolder userFolder);

        void Add(UserFolder userFolder);

        void Update(UserFolder userFolder);

        void SaveChanges();

        UserFolder GetMainUserFolder();

        bool IsGameFolder(string folderPath);
    }
}