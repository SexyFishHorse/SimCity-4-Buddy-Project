namespace NIHEI.SC4Buddy.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Security.Policy;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Plugin : ModelBase
    {
        public Plugin()
        {
            PluginFiles = new Collection<PluginFile>();
        }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string Author { get; set; }

        [JsonProperty]
        public string Description { get; set; }

        [JsonProperty]
        public Url Link { get; set; }

        [JsonProperty]
        public ICollection<PluginFile> PluginFiles { get; set; }

        [JsonProperty]
        public string Group
        {
            get
            {
                return PluginGroup != null ? PluginGroup.Name : null;
            }
        }

        [JsonProperty]
        public Guid RemotePluginId
        {
            get
            {
                return RemotePlugin == null ? Guid.Empty : RemotePlugin.Id;
            }
        }

        public PluginGroup PluginGroup { get; set; }

        public Asser.Sc4Buddy.Server.Api.Client.V1.Models.Plugin RemotePlugin { get; set; }

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
    }
}
