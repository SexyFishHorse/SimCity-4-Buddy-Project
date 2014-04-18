namespace Sc4BuddyServer.Models
{
    using System;

    public class PluginFile
    {
        public Guid Id { get; set; }

        public string Filename { get; set; }

        public string Md5Checksum { get; set; }
    }
}