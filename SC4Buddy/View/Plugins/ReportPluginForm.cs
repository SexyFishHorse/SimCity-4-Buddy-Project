namespace NIHEI.SC4Buddy.View.Plugins
{
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
        }
    }
}
