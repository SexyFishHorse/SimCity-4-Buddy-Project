namespace NIHEI.SC4Buddy.Plugins.View
{
    using System;
    using System.Windows.Forms;
    using Asser.Sc4Buddy.Server.Api.V1.Models;
    using NIHEI.SC4Buddy.Entities.Remote;

    public partial class ReportPluginForm : Form
    {

        public ReportPluginForm()
        {
            InitializeComponent();
        }

        public Plugin Plugin { get; set; }

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
