namespace NIHEI.SC4Buddy.UserFolders.View
{
    using System.Windows.Forms;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.UserFolders.Control;
    using NIHEI.SC4Buddy.View.Elements;

    public partial class SelectUserFolderForm : Form
    {
        public SelectUserFolderForm(IUserFoldersController userFoldersController)
        {
            InitializeComponent();

            userFolderListView.BeginUpdate();
            foreach (var userFolder in userFoldersController.UserFolders)
            {
                userFolderListView.Items.Add(new ListViewItemWithObjectValue<UserFolder>(userFolder.Alias, userFolder));
            }

            userFolderListView.EndUpdate();
        }

        public UserFolder UserFolder { get; set; }

        private void InstallButtonClick(object sender, System.EventArgs e)
        {
            UserFolder = ((ListViewItemWithObjectValue<UserFolder>)userFolderListView.SelectedItems[0]).Value;
        }

        private void UserFolderListViewSelectedIndexChanged(object sender, System.EventArgs e)
        {
            installButton.Enabled = userFolderListView.SelectedItems.Count > 0;
        }
    }
}
