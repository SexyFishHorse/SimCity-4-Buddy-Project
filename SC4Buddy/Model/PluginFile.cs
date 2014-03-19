using System;
using System.Collections.Generic;

namespace NIHEI.SC4Buddy.Model
{
    public class PluginFile
    {
        public Guid Id { get; set; }

        public string Path { get; set; }

        public string Checksum { get; set; }

        public Guid PluginId { get; set; }

        public PluginFile(Guid id)
        {
            Id = id;
        }

        private bool Equals(PluginFile other)
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
            return Equals((PluginFile)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private sealed class PathEqualityComparer : IEqualityComparer<PluginFile>
        {
            public bool Equals(PluginFile x, PluginFile y)
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
                return string.Equals(x.Path, y.Path);
            }

            public int GetHashCode(PluginFile obj)
            {
                return obj.Path.GetHashCode();
            }
        }

        private static readonly IEqualityComparer<PluginFile> PathComparerInstance = new PathEqualityComparer();

        public static IEqualityComparer<PluginFile> PathComparer
        {
            get
            {
                return PathComparerInstance;
            }
        }
    }
}
