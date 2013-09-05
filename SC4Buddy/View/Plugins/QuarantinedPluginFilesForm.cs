using NIHEI.SC4Buddy.Entities;

namespace NIHEI.SC4Buddy.View.Plugins
{
    using System;
    using System.Windows.Forms;

    public partial class QuarantinedPluginFilesForm : Form
    {
        private readonly Plugin _selectedPlugin;

        public QuarantinedPluginFilesForm(Plugin selectedPlugin)
        {
            _selectedPlugin = selectedPlugin;

            InitializeComponent();
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
