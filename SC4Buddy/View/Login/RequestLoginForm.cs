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
            var userController = new UserController(RemoteRegistryFactory.UserRegistry, RemoteRegistryFactory.AuthorRegistry);

            try
            {
                userController.CreateUser(
                    emailTextBox.Text.Trim(),
                    passwordTextBox.Text,
                    repeatPasswordTextBox.Text,
                    siteUrlTextBox.Text.Trim(),
                    usernameTextBox.Text.Trim());

                MessageBox.Show(
                    this,
                    LocalizationStrings.YouWillReceiveAnEmailWhenYourUserHasBeenValidatedAndActivated,
                    LocalizationStrings.RequestSent,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);

                ClearForm();

                Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    LocalizationStrings.RegistrationError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
            }
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            ClearForm();
            Close();
        }

        private void ClearForm()
        {
            emailTextBox.Text = string.Empty;
            passwordTextBox.Text = string.Empty;
            repeatPasswordTextBox.Text = string.Empty;
            siteUrlTextBox.Text = string.Empty;
            usernameTextBox.Text = string.Empty;
        }
    }
}
