namespace NIHEI.SC4Buddy.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NIHEI.SC4Buddy.Entities;

    public interface IUserFolderRegistry
    {
        ICollection<UserFolder> UserFolders { get; }

        void Add(UserFolder userFolder);

        void Update(UserFolder userFolder);

        void Delete(UserFolder userFolder);
    }

    public class UserFolderRegistry : IUserFolderRegistry
    {
        private readonly DatabaseEntities entities;

        public UserFolderRegistry(DatabaseEntities databaseEntities)
        {
            entities = databaseEntities;
        }

        public ICollection<UserFolder> UserFolders
        {
            get
            {
                return entities.UserFolders.ToList();
            }
        }

        public void Add(UserFolder userFolder)
        {
            entities.UserFolders.AddObject(userFolder);
            entities.SaveChanges();
        }

        public void Update(UserFolder userFolder)
        {
            if (userFolder.Id < 1)
            {
                throw new ArgumentException(@"Plugin folder must have an id", "userFolder");
            }

            if (entities.UserFolders.Any(x => x.Id == userFolder.Id))
            {
                entities.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException(
                    "Plugin folder not in collection (did you change the id or forgot to add it first?)");
            }
        }

        public void Delete(UserFolder userFolder)
        {
            entities.UserFolders.DeleteObject(userFolder);
            entities.SaveChanges();
        }
    }
}
