namespace NIHEI.SC4Buddy.View.Plugins
{
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Entities.Remote;

    public partial class ReportPluginForm : Form
    {
        public ReportPluginForm()
        {
            InitializeComponent();
        }

        public RemotePlugin Plugin { get; set; }
    }
}
