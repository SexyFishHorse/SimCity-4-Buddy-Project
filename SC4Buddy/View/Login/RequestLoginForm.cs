namespace NIHEI.SC4Buddy.View.Login
{
    using System;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Control;
    using NIHEI.SC4Buddy.DataAccess.Remote;
    using NIHEI.SC4Buddy.Localization;

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
                LocalizationStrings.SiteUrlHelpText,
                LocalizationStrings.SiteUrlHelp,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
        }

        private void UsernamePictureBoxClick(object sender, EventArgs e)
        {
            MessageBox.Show(
                this,
                LocalizationStrings.UsernameHelpText,
                LocalizationStrings.UsernameHelp,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
        }

        private void RequestButtonClick(object sender, EventArgs e)
        {
            var userController = new UserController(RemoteRegistryFactory.UserRegistry);

            userController.CreateUser(
                emailTextBox.Text.Trim(), passwordTextBox.Text, siteUrlTextBox.Text.Trim(), usernameTextBox.Text.Trim());

            MessageBox.Show(
                this,
                LocalizationStrings.YouWillReceiveAnEmailWhenYourUserHasBeenValidatedAndActivated,
                LocalizationStrings.RequestSent,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);
        }
    }
}
