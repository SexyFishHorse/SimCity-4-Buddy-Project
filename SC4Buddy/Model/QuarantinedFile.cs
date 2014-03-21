namespace NIHEI.SC4Buddy.Model
{
    using System;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class QuarantinedFile : ModelBase
    {
        [JsonProperty]
        public string QuarantinedPath { get; set; }

        public PluginFile File { get; set; }

        [JsonProperty]
        public Guid FileId
        {
            get
            {
                return File.Id;
            }
        }

        public QuarantinedFile(Guid id)
            : base(id)
        {
        }
    }
}
