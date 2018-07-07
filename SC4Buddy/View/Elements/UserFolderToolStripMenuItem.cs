namespace Nihei.SC4Buddy.View.Elements
{
    using System;
    using System.Windows.Forms;
    using Nihei.SC4Buddy.Model;

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
