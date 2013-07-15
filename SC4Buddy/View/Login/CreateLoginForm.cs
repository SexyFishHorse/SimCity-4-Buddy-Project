﻿namespace NIHEI.SC4Buddy.View.Login
{
    using System;
    using System.ComponentModel.DataAnnotations;
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

        private void RequestButtonClick(object sender, EventArgs e)
        {
            var userController = new UserController(RemoteRegistryFactory.UserRegistry);

            try
            {
                userController.CreateUser(
                    emailTextBox.Text.Trim(),
                    passwordTextBox.Text,
                    repeatPasswordTextBox.Text);

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
            catch (ValidationException ex)
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
        }
    }
}
