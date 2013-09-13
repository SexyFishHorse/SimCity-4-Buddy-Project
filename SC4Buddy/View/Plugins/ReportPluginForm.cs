namespace NIHEI.SC4Buddy.View.Plugins
{
    using System;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Control.Remote;
    using NIHEI.SC4Buddy.Entities.Remote;

    public partial class ReportPluginForm : Form
    {
        private readonly RemotePluginController remotePluginController;

        public ReportPluginForm(RemotePluginController remotePluginController)
        {
            this.remotePluginController = remotePluginController;
            InitializeComponent();
        }

        public RemotePlugin Plugin { get; set; }

        private void CancelButtonClick(object sender, System.EventArgs e)
        {
            Close();
        }

        private void ReportButtonClick(object sender, System.EventArgs e)
        {
            var report = new PluginReport { Approved = false, Body = reportTextBox.Text.Trim(), Date = DateTime.UtcNow };

            Plugin.Reports.Add(report);

            remotePluginController.SaveChanges();

            Close();
        }

        private void ReportTextBoxTextChanged(object sender, System.EventArgs e)
        {
            reportButton.Enabled = reportTextBox.Text.Trim().Length > 0;
        }
    }
}
