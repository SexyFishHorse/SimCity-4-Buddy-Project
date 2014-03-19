using System;
using System.Collections.Generic;

namespace NIHEI.SC4Buddy.Model
{
    public class PluginGroup
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Guid> PluginIds { get; set; }

        public PluginGroup(Guid id)
        {
            Id = id;
        }

        protected bool Equals(PluginGroup other)
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
            return Equals((PluginGroup)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private sealed class NameEqualityComparer : IEqualityComparer<PluginGroup>
        {
            public bool Equals(PluginGroup x, PluginGroup y)
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

            public int GetHashCode(PluginGroup obj)
            {
                return (obj.Name != null ? obj.Name.GetHashCode() : 0);
            }
        }

        private static readonly IEqualityComparer<PluginGroup> NameComparerInstance = new NameEqualityComparer();

        public static IEqualityComparer<PluginGroup> NameComparer
        {
            get
            {
                return NameComparerInstance;
            }
        }
    }
}
