namespace Nihei.SC4Buddy.UserFolders.Control
{
    using System;
    using System.Collections.Generic;
    using Nihei.SC4Buddy.Model;

    public interface IUserFoldersController
    {
        ICollection<UserFolder> UserFolders { get; set; }

        UserFolder GetMainUserFolder();

        void Add(UserFolder userFolder);

        void Update(UserFolder userFolder);

        void Delete(UserFolder userFolder);

        bool ValidatePath(string path, Guid currentId);

        bool IsNotGameFolder(string path);

        bool ValidateAlias(string alias, Guid currentId);

        UserFolder GetUserFolderDataByPath(string path);
    }
}