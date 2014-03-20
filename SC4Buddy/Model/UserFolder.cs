namespace NIHEI.SC4Buddy.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UserFolder : ModelBase
    {
        public string FolderPath { get; set; }

        public string Alias { get; set; }

        public IEnumerable<Guid> PluginIds
        {
            get
            {
                return Plugins.Select(x => x.Id);
            }
        }

        public ICollection<Plugin> Plugins { get; set; }

        public UserFolder(Guid id)
            : base(id)
        {
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

        public string PluginFolderPath
        {
            get
            {
                return System.IO.Path.Combine(FolderPath, "Plugins");
            }
        }
    }
}
