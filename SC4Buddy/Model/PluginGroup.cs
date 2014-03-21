﻿using System;
using System.Collections.Generic;

namespace NIHEI.SC4Buddy.Model
{
    using System.Linq;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class PluginGroup : ModelBase
    {
        [JsonProperty]
        public string Name { get; set; }

        public ICollection<Plugin> Plugins { get; set; }

        [JsonProperty]
        public IEnumerable<Guid> PluginIds
        {
            get
            {
                return Plugins.Select(x => x.Id);
            }
        }

        public PluginGroup(Guid id)
            : base(id)
        {
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
