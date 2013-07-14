namespace NIHEI.SC4Buddy.View.Elements
{
    using System.Globalization;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Entities;

    public class UserFolderListViewItem : ListViewItem
    {
        public UserFolderListViewItem(UserFolder userFolder)
        {
            Text = userFolder.Alias;
            Name = userFolder.Id.ToString(CultureInfo.InvariantCulture);
            UserFolder = userFolder;
        }

        public UserFolder UserFolder { get; set; }
    }
}
