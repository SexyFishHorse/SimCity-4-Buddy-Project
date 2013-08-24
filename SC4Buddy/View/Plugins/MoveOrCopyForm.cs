using System.Windows.Forms;

namespace NIHEI.SC4Buddy.View.Plugins
{
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.View.Elements;

    public partial class MoveOrCopyForm : Form
    {
        private UserFolder selectedUserFolder;

        public MoveOrCopyForm()
        {
            InitializeComponent();
        }

        public Plugin Plugin { get; set; }

        private void CancelButtonClick(object sender, System.EventArgs e)
        {
            Close();
        }

        private void MoveOrCopyFormLoad(object sender, System.EventArgs e)
        {
            userFolderListView.BeginUpdate();
            userFolderListView.Items.Clear();

            var userFolders = RegistryFactory.UserFolderRegistry.UserFolders;

            foreach (var userFolder in userFolders)
            {
                userFolderListView.Items.Add(new ListViewItemWithObjectValue<UserFolder>(userFolder.Alias, userFolder));
            }

            userFolderListView.EndUpdate();
        }

        private void UserFolderListViewSelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (userFolderListView.SelectedItems.Count > 0)
            {
                selectedUserFolder = ((ListViewItemWithObjectValue<UserFolder>)userFolderListView.SelectedItems[0]).Value;

                moveButton.Enabled = true;
                copyButton.Enabled = true;
            }
            else
            {
                moveButton.Enabled = false;
                copyButton.Enabled = false;
            }
        }
    }
}
