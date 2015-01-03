namespace NIHEI.SC4Buddy.View.Plugins
{
    using System;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Entities.Remote;

    using RemotePlugin = Irradiated.Sc4Buddy.ApiClient.Model.Plugin;
    using RemotePluginFile = Irradiated.Sc4Buddy.ApiClient.Model.PluginFile;

    public partial class ReportPluginForm : Form
    {

        public ReportPluginForm()
        {
            InitializeComponent();
        }

        public RemotePlugin Plugin { get; set; }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void ReportButtonClick(object sender, EventArgs e)
        {
            var report = new PluginReport { Approved = false, Body = reportTextBox.Text.Trim(), Date = DateTime.UtcNow };

            throw new NotImplementedException();

            Close();
        }

        private void ReportTextBoxTextChanged(object sender, EventArgs e)
        {
            reportButton.Enabled = reportTextBox.Text.Trim().Length > 0;
        }
    }
}
