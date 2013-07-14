namespace NIHEI.SC4Buddy.View.Elements
{
    using System;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Entities;

    public class UserFolderToolStripMenuItem : ToolStripMenuItem
    {
        public UserFolderToolStripMenuItem(UserFolder userFolder, EventHandler onClick)
            : base(userFolder.Alias, null, onClick)
        {
            UserFolder = userFolder;
        }

        public UserFolder UserFolder { get; set; }
    }
}
