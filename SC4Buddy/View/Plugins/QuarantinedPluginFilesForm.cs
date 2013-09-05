namespace NIHEI.SC4Buddy.View.Plugins
{
    using System;
    using System.Windows.Forms;

    public partial class QuarantinedPluginFilesForm : Form
    {
        public QuarantinedPluginFilesForm()
        {
            InitializeComponent();
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
