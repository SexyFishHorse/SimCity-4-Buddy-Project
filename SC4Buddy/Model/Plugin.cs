﻿namespace NIHEI.SC4Buddy.Model
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Security.Policy;

    using Newtonsoft.Json;

    using RemotePlugin = Irradiated.Sc4Buddy.ApiClient.Model.Plugin;
    using RemotePluginFile = Irradiated.Sc4Buddy.ApiClient.Model.PluginFile;

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

        [JsonProperty]
        public RemotePlugin RemotePlugin { get; set; }

        public PluginGroup PluginGroup { get; set; }

        public ICollection<PluginFile> PluginFiles { get; set; }

        [JsonProperty]
        public Guid PluginGroupId
        {
            get
            {
                return PluginGroup != null ? PluginGroup.Id : Guid.Empty;
            }
        }

        [JsonProperty]
        public string PluginGroupName
        {
            get
            {
                return PluginGroup != null ? PluginGroup.Name : null;
            }
        }

        public Plugin()
        {
            PluginFiles = new Collection<PluginFile>();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Plugin))
            {
                return false;
            }

            return obj.GetHashCode() == GetHashCode();
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
