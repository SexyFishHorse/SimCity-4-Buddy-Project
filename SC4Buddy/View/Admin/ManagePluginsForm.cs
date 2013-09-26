namespace NIHEI.SC4Buddy.View.Admin
{
    using System;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Control.Remote;
    public partial class ManagePluginsForm : Form
    {
        private readonly RemotePluginController remotePluginController;

        public ManagePluginsForm(RemotePluginController remotePluginController)
        {
            this.remotePluginController = remotePluginController;
            InitializeComponent();
        }

        private void SearchComboBoxValueChanged(object sender, EventArgs e)
        {
        }
    }
}
