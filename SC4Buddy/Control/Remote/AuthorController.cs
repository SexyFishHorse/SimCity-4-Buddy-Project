namespace NIHEI.SC4Buddy.Control.Remote
{
    using System;
    using System.Data.Objects;
    using System.Linq;

    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities.Remote;

    public class AuthorController
    {
        private readonly IRemoteEntities entities;

        public AuthorController(IRemoteEntities entities)
        {
            this.entities = entities;
        }

        public IObjectSet<Author> Authors
        {
            get
            {
                return entities.Authors;
            }
        }

        public Author GetAuthorByName(string authorName)
        {
            if (!Authors.Any(x => x.Name.Equals(authorName, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }

            return Authors.First(x => x.Name.Equals(authorName, StringComparison.OrdinalIgnoreCase));
        }

        public void Add(Author author)
        {
            entities.Authors.AddObject(author);
            SaveChanges();
        }

        public void Delete(Author author)
        {
            entities.Authors.DeleteObject(author);
            SaveChanges();
        }

        public void SaveChanges()
        {
            entities.SaveChanges();
        }
    }
}
