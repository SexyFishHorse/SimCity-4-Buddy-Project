namespace NIHEI.SC4Buddy.Model
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class UserFolder : ModelBase
    {
        public const string PluginFolderName = "Plugins";

        private static readonly IEqualityComparer<UserFolder> AliasComparerInstance = new AliasEqualityComparer();

        public UserFolder()
        {
            Plugins = new Collection<Plugin>();
        }

        public static IEqualityComparer<UserFolder> AliasComparer
        {
            get
            {
                return AliasComparerInstance;
            }
        }

        [JsonProperty]
        public string FolderPath { get; set; }

        [JsonProperty]
        public string Alias { get; set; }

        [JsonProperty]
        public bool IsMainFolder { get; set; }

        [JsonProperty]
        public bool IsStartupFolder { get; set; }

        public ICollection<Plugin> Plugins { get; set; }

        public string PluginFolderPath
        {
            get
            {
                return System.IO.Path.Combine(FolderPath, PluginFolderName);
            }
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
                return obj.Alias != null ? obj.Alias.GetHashCode() : 0;
            }
        }
    }
}
