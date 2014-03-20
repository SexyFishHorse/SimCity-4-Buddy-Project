using System;
using System.Collections.Generic;

namespace NIHEI.SC4Buddy.Model
{
    using System.Linq;
    using System.Security.Policy;

    using NIHEI.SC4Buddy.Entities.Remote;

    public class Plugin : ModelBase
    {
        public string Name { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public Guid PluginGroupId
        {
            get
            {
                return Group.Id;
            }
        }

        public Url Link { get; set; }

        public Guid UserFolderId
        {
            get
            {
                return UserFolder.Id;
            }
        }

        public Guid RemotePluginId
        {
            get
            {
                return RemotePlugin.Id;
            }
        }

        public UserFolder UserFolder { get; set; }

        public RemotePlugin RemotePlugin { get; set; }

        public PluginGroup Group { get; set; }

        public ICollection<PluginFile> Files { get; set; }

        public IEnumerable<Guid> PluginFileIds
        {
            get
            {
                return Files.Select(x => x.Id);
            }
        }

        public Plugin(Guid id)
            : base(id)
        {
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
