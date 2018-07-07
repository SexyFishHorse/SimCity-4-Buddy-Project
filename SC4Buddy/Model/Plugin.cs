namespace Nihei.SC4Buddy.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Plugin : ModelBase
    {
        public Plugin(Guid? id = null) : base(id)
        {
        }

        [JsonProperty]
        public string Author { get; set; }

        [JsonProperty]
        public string Description { get; set; }

        [JsonProperty]
        public string Group => PluginGroup?.Name;

        [JsonProperty]
        public string Link { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public ICollection<PluginFile> PluginFiles { get; set; } = new Collection<PluginFile>();

        public PluginGroup PluginGroup { get; set; }

        public Asser.Sc4Buddy.Server.Api.V1.Models.Plugin RemotePlugin { get; set; }

        [JsonProperty]
        public Guid? RemotePluginId => RemotePlugin?.Id;
    }
}
