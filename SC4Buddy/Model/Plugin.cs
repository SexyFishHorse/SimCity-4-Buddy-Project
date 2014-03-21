using System;
using System.Collections.Generic;

namespace NIHEI.SC4Buddy.Model
{
    using System.Linq;
    using System.Security.Policy;

    using Newtonsoft.Json;

    using NIHEI.SC4Buddy.Entities.Remote;

    [JsonObject(MemberSerialization.OptIn)]
    public class Plugin : ModelBase
    {
        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string Author { get; set; }

        [JsonProperty]
        public string Description { get; set; }

        [JsonProperty]
        public Url Link { get; set; }

        public UserFolder UserFolder { get; set; }

        public RemotePlugin RemotePlugin { get; set; }

        public PluginGroup Group { get; set; }

        public ICollection<PluginFile> Files { get; set; }

        [JsonProperty]
        public Guid UserFolderId
        {
            get
            {
                return UserFolder.Id;
            }
        }

        [JsonProperty]
        public int RemotePluginId
        {
            get
            {
                return RemotePlugin.Id;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        [JsonProperty]
        public Guid PluginGroupId
        {
            get
            {
                return Group.Id;
            }
        }

        [JsonProperty]
        public IEnumerable<Guid> PluginFileIds
        {
            get
            {
                return Files.Select(x => x.Id);
            }
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
