namespace NIHEI.SC4Buddy.View.Author
{
    using System;
    using System.Windows.Forms;

    public partial class AddPluginInformationForm : Form
    {
        public AddPluginInformationForm()
        {
            InitializeComponent();
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
