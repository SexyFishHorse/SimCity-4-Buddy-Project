namespace NIHEI.SC4Buddy.View.Login
{
    using System;
    using System.Windows.Forms;

    public partial class RequestLoginForm : Form
    {
        public RequestLoginForm()
        {
            InitializeComponent();
        }

        private void SitePictureBoxClick(object sender, EventArgs e)
        {
            MessageBox.Show(
                this,
                "The URL to the website where you have uploaded your content."
                + " (Enter only 1 URL, you can add more sites when your user has been created)",
                "Site URL help",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
        }

        private void UsernamePictureBoxClick(object sender, EventArgs e)
        {
            MessageBox.Show(
                this,
                "Your username (what others see and search after) on the site you entered above.",
                "Username help",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
        }

        private void RequestButtonClick(object sender, EventArgs e)
        {
            MessageBox.Show(
                this,
                "You will receive an e-mail when your user has been validated and activated.",
                "Request sent",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
        }
    }
}
