namespace Sc4BuddyServer.Models
{
    using System;
    using System.Collections.Generic;

    public class Plugin
    {
        public Guid Id { get; set; }

        public Version Version { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string LinkToDownloadPage { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime DateModified { get; set; }

        public IEnumerable<PluginFile> Files { get; set; }
    }
}