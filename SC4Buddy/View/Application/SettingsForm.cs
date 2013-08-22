namespace NIHEI.SC4Buddy.View.Application
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using System.Security.Authentication;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Control;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Properties;
    using NIHEI.SC4Buddy.View.Elements;
    using NIHEI.SC4Buddy.View.Login;

    using log4net;

    public partial class SettingsForm : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISettingsController settingsController;

        public SettingsForm()
        {
            settingsController = new SettingsController(RegistryFactory.UserFolderRegistry);

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
                Log.Info(string.Format("Browsed to valid game location: {0}", path));
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
                Log.Info("OK pressed, no valid game folder set.");
                MessageBox.Show(
                    this,
                    LocalizationStrings.InvalidGameLocationFolder,
                    LocalizationStrings.GameNotFound,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (backgroundImageListView.SelectedIndices.Count > 0)
            {
                Settings.Default.Wallpaper = backgroundImageListView.SelectedIndices[0] + 1;
            }

            Settings.Default.Save();

            settingsController.UpdateMainFolder();

            Close();
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Log.Info("Closing settings form");
            Close();
        }

        private void ScanButtonClick(object sender, EventArgs e)
        {
            var gameLocation = settingsController.SearchForGameLocation();

            if (string.IsNullOrWhiteSpace(gameLocation))
            {
                Log.Info("Could not find game location using the scanner");

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
                Log.Info("Game found using scanner");

                gameLocationTextBox.Text = gameLocation;
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

            cpuCountComboBox.BeginUpdate();
            cpuCountComboBox.Items.Clear();
            cpuCountComboBox.Items.Add(LocalizationStrings.Ignore);
            for (var i = 1; i <= Environment.ProcessorCount; i++)
            {
                cpuCountComboBox.Items.Add(i);
            }

            int selectedIndex;
            int.TryParse(Settings.Default.LauncherCpuCount, out selectedIndex);
            cpuCountComboBox.SelectedIndex = selectedIndex;
            cpuCountComboBox.EndUpdate();

            cpuPriorityComboBox.BeginUpdate();
            cpuPriorityComboBox.Items.Clear();
            cpuPriorityComboBox.Items.Add(LocalizationStrings.Ignore);
            cpuPriorityComboBox.Items.Add(new ComboBoxItem<GameArgumentsController.CpuPriority>(LocalizationStrings.Low, GameArgumentsController.CpuPriority.Low));
            cpuPriorityComboBox.Items.Add(new ComboBoxItem<GameArgumentsController.CpuPriority>(LocalizationStrings.Medium, GameArgumentsController.CpuPriority.Medium));
            cpuPriorityComboBox.Items.Add(new ComboBoxItem<GameArgumentsController.CpuPriority>(LocalizationStrings.High, GameArgumentsController.CpuPriority.High));
            cpuPriorityComboBox.EndUpdate();

            cursorColourComboBox.BeginUpdate();
            cpuCountComboBox.Items.Clear();
            cpuCountComboBox.Items.Add(LocalizationStrings.Ignore);
            cpuCountComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsController.CursorColorDepth>(
                    LocalizationStrings.Disabled, GameArgumentsController.CursorColorDepth.Disabled));
            cpuCountComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsController.CursorColorDepth>(
                    LocalizationStrings.SystemCursors, GameArgumentsController.CursorColorDepth.SystemCursors));
            cpuCountComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsController.CursorColorDepth>(
                    LocalizationStrings.BlackAndWhite, GameArgumentsController.CursorColorDepth.BlackAndWhite));
            cpuCountComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsController.CursorColorDepth>(
                    LocalizationStrings.Colors16, GameArgumentsController.CursorColorDepth.Colors16));
            cpuCountComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsController.CursorColorDepth>(
                    LocalizationStrings.Colors256, GameArgumentsController.CursorColorDepth.Colors256));
            cpuCountComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsController.CursorColorDepth>(
                    LocalizationStrings.FullColors, GameArgumentsController.CursorColorDepth.FullColors));
            cpuCountComboBox.EndUpdate();

            renderModeComboBox.SelectedIndex = 1;
            colourDepthComboBox.SelectedIndex = 1;
            cursorColourComboBox.SelectedIndex = 5;

            UpdateLanguageBox();

            UpdateBackgroundsListView();

            UpdateLoginStatus();
        }

        private void UpdateBackgroundsListView()
        {
            var wallpapers = settingsController.GetWallpapers();

            backgroundImageListView.BeginUpdate();
            var imageList = new ImageList { ImageSize = new Size(65, 65) };
            backgroundImageListView.LargeImageList = imageList;

            for (var index = 0; index < wallpapers.Count; index++)
            {
                var wallpaper = wallpapers[index];
                var item = new ListViewItem((index + 1).ToString(CultureInfo.InvariantCulture));
                imageList.Images.Add(wallpaper);
                item.ImageIndex = index;

                backgroundImageListView.Items.Add(item);
            }

            backgroundImageListView.EndUpdate();
        }

        private void UpdateLoginStatus()
        {
            emailTextBox.Text = string.Empty;
            passwordTextBox.Text = string.Empty;

            if (SessionController.Instance.IsLoggedIn)
            {
                Log.Info("User is logged in");

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
                Log.Info("User is not logged in");

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

            var languages = settingsController.GetInstalledLanguages();

            languageComboBox.BeginUpdate();
            languageComboBox.Items.Clear();
            languageComboBox.Items.AddRange(languages.Cast<object>().ToArray());
            languageComboBox.EndUpdate();
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

            if (!settingsController.ValidateGameLocationPath(Settings.Default.GameLocation))
            {
                var result = MessageBox.Show(
                this,
                LocalizationStrings.YouMustSettheGameLocationBeforeYouCanUseThisApplication,
                LocalizationStrings.GameFolderNotSet,
                MessageBoxButtons.RetryCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1);

                if (result != DialogResult.Retry)
                {
                    return;
                }

                Log.Info("Abort form close");
                e.Cancel = true;
            }
        }

        private void LoginButtonClick(object sender, EventArgs e)
        {
            try
            {
                Log.Info("Logging in");
                SessionController.Instance.Login(emailTextBox.Text, passwordTextBox.Text);
                UpdateLoginStatus();
            }
            catch (InvalidCredentialException ex)
            {
                Log.Error("error on login", ex);

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
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                return;
            }

            Log.Info("No internet connection detected");
            MessageBox.Show(
                this,
                LocalizationStrings.YouDoNotAppearToBeConnectedToTheInternet,
                LocalizationStrings.NoInternetDetectionDetected,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1);
        }

        private void EnableRemoteDatabaseConnectionCheckboxCheckedChanged(object sender, EventArgs e)
        {
            if (enableRemoteDatabaseConnectionCheckbox.Checked)
            {
                fetchInformationFromRemoteCheckbox.Enabled = true;
                allowCheckMissingDependenciesCheckBox.Enabled = true;
            }
            else
            {
                fetchInformationFromRemoteCheckbox.Enabled = false;
                allowCheckMissingDependenciesCheckBox.Enabled = false;
            }
        }
    }
}
