namespace NIHEI.SC4Buddy.Model
{
    using System;
    using System.Collections.Generic;

    public class UserFolder
    {
        public UserFolder(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }

        public string Path { get; set; }

        public string Alias { get; set; }

        public IEnumerable<Guid> PluginIds { get; set; } 

        private bool Equals(UserFolder other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((UserFolder)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private sealed class AliasEqualityComparer : IEqualityComparer<UserFolder>
        {
            public bool Equals(UserFolder x, UserFolder y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }
                if (ReferenceEquals(x, null))
                {
                    return false;
                }
                if (ReferenceEquals(y, null))
                {
                    return false;
                }
                if (x.GetType() != y.GetType())
                {
                    return false;
                }
                return string.Equals(x.Alias, y.Alias);
            }

            public int GetHashCode(UserFolder obj)
            {
                return (obj.Alias != null ? obj.Alias.GetHashCode() : 0);
            }
        }

        private static readonly IEqualityComparer<UserFolder> AliasComparerInstance = new AliasEqualityComparer();

        public static IEqualityComparer<UserFolder> AliasComparer
        {
            get
            {
                return AliasComparerInstance;
            }
        }
    }
}
