namespace NIHEI.SC4Buddy.Control.UserFolders
{
    using System;
    using System.Collections.Generic;
    using NIHEI.SC4Buddy.Model;

    public interface IUserFolderRepository
    {
        IEnumerable<UserFolder> UserFolders { get; }

        void Delete(UserFolder userFolder);

        void Add(UserFolder userFolder);

        void Update(UserFolder userFolder);

        UserFolder GetMainUserFolder();

        void SaveChanges();

        /// <summary>
        /// Validates that the specified path is not empty or a whitespace 
        /// and that the path exist on the local machine.
        /// It also checks if the path is already in use in 
        /// </summary>
        /// <param name="path">The path to validate.</param>
        /// <param name="currentId">The id of the object to skip when checking for uniqueness.</param>
        /// <returns>TRUE if the path complies with the above rules.</returns>
        bool ValidatePath(string path, Guid currentId);

        /// <summary>
        /// Validates that the specified alias is not empty or a whitespace
        /// and that the alias is not already in use.
        /// </summary>
        /// <param name="alias">The alias to validate.</param>
        /// <param name="currentId">The id of the object to skip when checking for uniqueness.</param>
        /// <returns>TRUE if the alias complies with the above rules.</returns>
        bool ValidateAlias(string alias, Guid currentId);

        bool IsNotGameFolder(string path);
    }
}
