namespace NIHEI.SC4Buddy.DataAccess.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NIHEI.SC4Buddy.Entities.Remote;

    public class AuthorRegistry
    {
        private readonly RemoteEntities entities;

        public AuthorRegistry(RemoteEntities entities)
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
            var authorRegistry = RemoteRegistryFactory.AuthorRegistry;

            if (!authorRegistry.Authors.Any(x => x.Name.Equals(authorName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException(@"No author by that name exists.", "authorName");
            }

            return authorRegistry.Authors.First(x => x.Name.Equals(authorName, StringComparison.OrdinalIgnoreCase));
        }

        public void Add(Author author)
        {
            entities.Authors.AddObject(author);
            entities.SaveChanges();
        }

        public void Delete(Author author)
        {
            entities.Authors.DeleteObject(author);
            entities.SaveChanges();
        }
    }
}
