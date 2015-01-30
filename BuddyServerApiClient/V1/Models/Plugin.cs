namespace Asser.Sc4Buddy.Server.Api.Client.V1.Models
{
    using System;
    using System.Collections.Generic;

    public class Plugin
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }

        public string Link { get; set; }

        public string Description { get; set; }

        public IEnumerable<File> Files { get; set; }
    }
}
