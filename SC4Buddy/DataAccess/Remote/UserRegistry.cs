namespace NIHEI.SC4Buddy.DataAccess.Remote
{
    using System.Collections.Generic;

    using NIHEI.SC4Buddy.Entities.Remote;

    public class UserRegistry
    {
        private readonly RemoteDatabaseEntities entities;

        public UserRegistry(RemoteDatabaseEntities entities)
        {
            this.entities = entities;
        }

        public IEnumerable<User> Users
        {
            get
            {
                return entities.Users;
            }
        }

        public void Add(User user)
        {
            entities.Users.AddObject(user);
            entities.SaveChanges();
        }

        public void Update(User user)
        {
            entities.SaveChanges();
        }
    }
}
