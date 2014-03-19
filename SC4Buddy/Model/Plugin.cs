using System;
using System.Collections.Generic;

namespace NIHEI.SC4Buddy.Model
{
    using System.Security.Policy;

    public class Plugin
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public Guid PluginGroupId { get; set; }

        public Url Link { get; set; }

        public Guid UserFolderId { get; set; }

        public Plugin(Guid id)
        {
            Id = id;
        }

        private bool Equals(Plugin other)
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
            return Equals((Plugin)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private sealed class NameEqualityComparer : IEqualityComparer<Plugin>
        {
            public bool Equals(Plugin x, Plugin y)
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
                return string.Equals(x.Name, y.Name);
            }

            public int GetHashCode(Plugin obj)
            {
                return (obj.Name != null ? obj.Name.GetHashCode() : 0);
            }
        }

        private static readonly IEqualityComparer<Plugin> NameComparerInstance = new NameEqualityComparer();

        public static IEqualityComparer<Plugin> NameComparer
        {
            get
            {
                return NameComparerInstance;
            }
        }
    }
}
