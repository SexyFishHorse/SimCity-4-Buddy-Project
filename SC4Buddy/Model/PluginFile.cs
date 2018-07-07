namespace Nihei.SC4Buddy.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class PluginFile : ModelBase
    {
        public PluginFile(Guid? id = null)
            : base(id)
        {
        }

        public static IEqualityComparer<PluginFile> PathComparer { get; } = new PathEqualityComparer();

        [JsonProperty]
        public string Checksum { get; set; }

        public string Filename => new FileInfo(Path).Name;

        [JsonProperty]
        public string Path { get; set; }

        [JsonProperty]
        public QuarantinedFile QuarantinedFile { get; set; }

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
    }
}
