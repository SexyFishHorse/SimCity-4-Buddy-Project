namespace NIHEI.SC4Buddy.UserFolders.Control
{
    using System.Collections.Generic;
    using System.Linq;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Model;

    public class UserFolderController : IUserFolderController
    {
        private readonly IEntities entities;

        public UserFolderController(
            IEntities entities)
        {
            this.entities = entities;
        }

        public IEnumerable<UserFolder> UserFolders
        {
            get
            {
                return entities.UserFolders;
            }
        }

        public void Update(UserFolder userFolder)
        {
            if (userFolder.IsStartupFolder)
            {
                foreach (var folder in UserFolders.Where(x => x.IsStartupFolder && x.Id != userFolder.Id))
                {
                    folder.IsStartupFolder = false;
                }
            }

            entities.SaveChanges();
        }

        public void SaveChanges()
        {
            entities.SaveChanges();
        }
    }
}
