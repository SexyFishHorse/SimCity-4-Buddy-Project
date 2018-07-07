namespace Nihei.SC4Buddy.Model
{
    using System;
    using System.Collections.Generic;

    public class NonPluginFileRemovalSummary
    {
        public int NumFilesRemoved { get; set; }

        public int NumFoldersRemoved { get; set; }

        public Dictionary<string, Exception> Errors { get; set; }
    }
}