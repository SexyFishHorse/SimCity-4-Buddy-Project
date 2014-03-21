using System;
using System.Collections.Generic;

namespace NIHEI.SC4Buddy.Model
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class PluginFile : ModelBase
    {
        [JsonProperty]
        public string Path { get; set; }

        [JsonProperty]
        public string Checksum { get; set; }

        public Plugin Plugin { get; set; }

        [JsonProperty]
        public Guid PluginId
        {
            get
            {
                return Plugin.Id;
            }
        }

        public PluginFile(Guid id)
            : base(id)
        {
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

        public QuarantinedFile QuarantinedFile { get; set; }
    }
}
