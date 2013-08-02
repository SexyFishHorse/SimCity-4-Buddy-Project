namespace NIHEI.SC4Buddy.View.Application
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Security.Authentication;
    using System.Windows.Forms;

    using Microsoft.Win32;

    using NIHEI.SC4Buddy.Control;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Properties;
    using NIHEI.SC4Buddy.View.Login;

    public partial class SettingsForm : Form
    {
        private readonly UserFolderRegistry userFolderRegistry;

        private readonly SettingsController settingsController;

        public SettingsForm()
        {
            userFolderRegistry = RegistryFactory.UserFolderRegistry;

            settingsController = new SettingsController();

            InitializeComponent();
        }

        private void BrowseButtonClick(object sender, EventArgs e)
        {
            var result = gameLocationDialog.ShowDialog(this);

            if (result == DialogResult.Cancel)
            {
                return;
            }

            var path = gameLocationDialog.SelectedPath;

            if (settingsController.ValidateGameLocationPath(path))
            {
                gameLocationTextBox.Text = path;
            }

            UpdateLanguageBox();
        }

        private void GameLocationTextBoxTextChanged(object sender, EventArgs e)
        {
            errorProvider.Clear();

            if (gameLocationTextBox.Text.Length < 1)
            {
                gameLocationTextBox.Text = LocalizationStrings.SelectGameLocation;
            }

            gameLocationTextBox.ForeColor = gameLocationTextBox.Text.Equals(
                LocalizationStrings.SelectGameLocation, StringComparison.OrdinalIgnoreCase)
                ? Color.Gray
                : Color.Black;

            UpdateLanguageBox();
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            if (!settingsController.ValidateGameLocationPath(gameLocationTextBox.Text))
            {
                MessageBox.Show(
                    this,
                    LocalizationStrings.InvalidGameLocationFolder,
                    LocalizationStrings.GameNotFound,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            if (backgroundImageListView.SelectedIndices.Count > 0)
            {
                Settings.Default.Wallpaper = backgroundImageListView.SelectedIndices[0] + 1;
            }

            Settings.Default.Save();

            UpdateMainFolder();

            Close();
        }

        private void UpdateMainFolder()
        {
            var folder = userFolderRegistry.UserFolders.FirstOrDefault(x => x.Id == 1);
            if (folder == null)
            {
                throw new InvalidOperationException("Main plugin folder has been deleted from the database.");
            }

            folder.Path = Settings.Default.GameLocation;
            folder.Alias = LocalizationStrings.GameUserFolderName;
            userFolderRegistry.Update(folder);
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void ScanButtonClick(object sender, EventArgs e)
        {
            var regKeys = new[]
                              {
                                  @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                                  @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
                              };

            var match = false;

            foreach (var regKey in regKeys)
            {
                using (var key = Registry.LocalMachine.OpenSubKey(regKey))
                {
                    if (key == null)
                    {
                        continue;
                    }

                    foreach (var subKeyName in key.GetSubKeyNames())
                    {
                        using (var subKey = key.OpenSubKey(subKeyName))
                        {
                            if (subKey == null)
                            {
                                continue;
                            }

                            if (string.IsNullOrWhiteSpace((string)subKey.GetValue("DisplayName")))
                            {
                                continue;
                            }

                            var name = (string)subKey.GetValue("DisplayName");
                            var path = (string)subKey.GetValue("InstallLocation");

                            if (!name.StartsWith("SimCity 4"))
                            {
                                continue;
                            }

                            if (!settingsController.ValidateGameLocationPath(path))
                            {
                                continue;
                            }

                            gameLocationTextBox.Text = path;
                            match = true;
                            break;
                        }
                    }
                    if (match)
                    {
                        break;
                    }
                }
            }

            if (!match)
            {
                MessageBox.Show(
                    this,
                    LocalizationStrings.UnableToLocateTheGameUseTheBrowseOptionInstead,
                    LocalizationStrings.GameNotFound,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
            }
            else
            {
                UpdateLanguageBox();
            }
        }

        private void SettingsFormLoad(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(gameLocationTextBox.Text))
            {
                gameLocationTextBox.Text = LocalizationStrings.SelectGameLocation;
                gameLocationTextBox.ForeColor = Color.Gray;
            }

            gameLocationTextBox.ForeColor = gameLocationTextBox.Text.Equals(
                LocalizationStrings.SelectGameLocation, StringComparison.OrdinalIgnoreCase)
                ? Color.Gray
                : Color.Black;

            autoSaveIntervalTrackBar.Enabled = Settings.Default.EnableAutoSave;
            UpdateAutoSaveLabel(Settings.Default.AutoSaveWaitTime);

            for (var i = 1; i <= Environment.ProcessorCount; i++)
            {
                cpuCountComboBox.Items.Add(i);
            }

            cpuCountComboBox.SelectedIndex = 0;
            displayModeComboBox.SelectedIndex = 1;
            renderModeComboBox.SelectedIndex = 1;
            colourDepthComboBox.SelectedIndex = 1;
            cursorColourComboBox.SelectedIndex = 5;

            UpdateLanguageBox();

            var wallpapers = new List<Bitmap>
                                 {
                                     Resources.Wallpaper1,
                                     Resources.Wallpaper2,
                                     Resources.Wallpaper3,
                                     Resources.Wallpaper4,
                                     Resources.Wallpaper5,
                                     Resources.Wallpaper6,
                                     Resources.Wallpaper7,
                                     Resources.Wallpaper8,
                                     Resources.Wallpaper9,
                                     Resources.Wallpaper10,
                                     Resources.Wallpaper11,
                                     Resources.Wallpaper12,
                                     Resources.Wallpaper13
                                 };

            var ilist = new ImageList { ImageSize = new Size(65, 65) };
            backgroundImageListView.LargeImageList = ilist;

            for (var index = 0; index < wallpapers.Count; index++)
            {
                var wallpaper = wallpapers[index];
                var lvi = new ListViewItem((index + 1).ToString(CultureInfo.InvariantCulture));
                ilist.Images.Add(wallpaper);
                lvi.ImageIndex = index;
                backgroundImageListView.Items.Add(lvi);
            }

            UpdateLoginStatus();
        }

        private void UpdateLoginStatus()
        {
            emailTextBox.Text = string.Empty;
            passwordTextBox.Text = string.Empty;
            if (SessionController.Instance.IsLoggedIn)
            {
                logoutButton.Enabled = true;
                loginButton.Enabled = false;
                createLoginButton.Enabled = false;
                emailTextBox.Text = SessionController.Instance.User.Email;
                emailTextBox.Enabled = false;
                passwordTextBox.Enabled = false;
                loginStatusLabel.Text = string.Format(LocalizationStrings.LoggedInAs, SessionController.Instance.User.Email);
            }
            else
            {
                logoutButton.Enabled = false;
                loginButton.Enabled = true;
                createLoginButton.Enabled = true;
                emailTextBox.Enabled = true;
                passwordTextBox.Enabled = true;
                loginStatusLabel.Text = LocalizationStrings.NoUserIsLoggedIn;
            }
        }

        private void UpdateLanguageBox()
        {
            if (!settingsController.ValidateGameLocationPath(Settings.Default.GameLocation))
            {
                return;
            }

            var dirs = Directory.EnumerateDirectories(
                Settings.Default.GameLocation, "*", SearchOption.TopDirectoryOnly);
            var languages =
                dirs.Select(
                    dir => new { dir, files = Directory.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly) })
                    .Where(
                        @t =>
                        @t.files.Any(file => file.EndsWith("SimCityLocale.dat", StringComparison.OrdinalIgnoreCase)))
                    .Select(@t => new DirectoryInfo(@t.dir).Name)
                    .ToList();

            languageComboBox.Items.AddRange(languages.Cast<object>().ToArray());
        }

        private void DisableAudioCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            disableMusicCheckBox.Enabled = !disableAudioCheckBox.Checked;
            disableSoundsCheckBox.Enabled = !disableAudioCheckBox.Checked;
        }

        private void EnableAutoSaveButtonCheckedChanged(object sender, EventArgs e)
        {
            autoSaveIntervalTrackBar.Enabled = enableAutoSaveButton.Checked;

            AutoSaveIntervalTrackBarScroll(sender, e);
        }

        private void AutoSaveIntervalTrackBarScroll(object sender, EventArgs e)
        {
            var interval = autoSaveIntervalTrackBar.Value;

            shortAutosaveIntervalsLabel.Visible = interval < 10;

            UpdateAutoSaveLabel(interval);
        }

        private void UpdateAutoSaveLabel(int interval)
        {
            autoSaveIntervalLabel.Text = string.Format(LocalizationStrings.NumMinutes, interval);
        }

        private void SettingsFormFormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Reload();

            if (settingsController.ValidateGameLocationPath(gameLocationTextBox.Text))
            {
                MessageBox.Show(
                    this,
                    LocalizationStrings.InvalidGameLocationFolder,
                    LocalizationStrings.GameNotFound,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var result = MessageBox.Show(
                this,
                LocalizationStrings.YouMustSettheGameLocationBeforeYouCanUseThisApplication,
                LocalizationStrings.GameFolderNotSet,
                MessageBoxButtons.RetryCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1);

            if (result == DialogResult.Retry)
            {
                e.Cancel = true;
            }
        }

        private void LoginButtonClick(object sender, EventArgs e)
        {
            try
            {
                SessionController.Instance.Login(emailTextBox.Text, passwordTextBox.Text);
                UpdateLoginStatus();
            }
            catch (InvalidCredentialException ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    LocalizationStrings.InvalidCredentials,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);

                passwordTextBox.Text = null;
            }
        }

        private void LogoutButtonClick(object sender, EventArgs e)
        {
            SessionController.Instance.Logout();
            UpdateLoginStatus();
        }

        private void CreateLoginButtonClick(object sender, EventArgs e)
        {
            new CreateLoginForm().ShowDialog(this);
        }

        private void TabPage3Click(object sender, EventArgs e)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show(
                    this,
                    LocalizationStrings.YouDoNotAppearToBeConnectedToTheInternet,
                    LocalizationStrings.NoInternetDetectionDetected,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
            }
        }
    }
}
