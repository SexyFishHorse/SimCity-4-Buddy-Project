using System.Windows.Forms;

namespace NIHEI.SC4Buddy.View.Plugins
{
    using NIHEI.SC4Buddy.Entities;

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
    }
}
