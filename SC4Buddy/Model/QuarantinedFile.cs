namespace NIHEI.SC4Buddy.Model
{
    using System;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class QuarantinedFile : ModelBase
    {
        [JsonProperty]
        public string QuarantinedPath { get; set; }

        public PluginFile PluginFile { get; set; }

        [JsonProperty]
        public Guid FileId
        {
            get
            {
                return PluginFile != null ? PluginFile.Id : Guid.Empty;
            }
        }
    }
}
