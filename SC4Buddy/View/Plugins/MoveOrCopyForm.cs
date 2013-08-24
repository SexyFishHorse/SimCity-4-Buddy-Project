using System.Windows.Forms;

namespace NIHEI.SC4Buddy.View.Plugins
{
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.View.Elements;

    public partial class MoveOrCopyForm : Form
    {
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
    }
}
