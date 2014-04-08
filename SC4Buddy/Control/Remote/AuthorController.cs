namespace NIHEI.SC4Buddy.Control.Remote
{
    using System;
    using System.Collections.Generic;
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

        public IEnumerable<Author> Authors
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
            entities.Authors.Add(author);
            SaveChanges();
        }

        public void Delete(Author author)
        {
            entities.Authors.Remove(author);
            SaveChanges();
        }

        public void SaveChanges()
        {
            entities.SaveChanges();
        }
    }
}
