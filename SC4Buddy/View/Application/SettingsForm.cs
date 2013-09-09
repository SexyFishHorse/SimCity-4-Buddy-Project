namespace NIHEI.SC4Buddy.View.Application
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using System.Security.Authentication;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using log4net;

    using NIHEI.SC4Buddy.Control;
    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Properties;
    using NIHEI.SC4Buddy.View.Elements;
    using NIHEI.SC4Buddy.View.Login;

    public partial class SettingsForm : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISettingsController settingsController;

        public SettingsForm(UserFolderController userFolderController)
        {
            InitializeComponent();

            settingsController = new SettingsController(userFolderController);

            var minOs = new Version(6, 2);
            if (Environment.OSVersion.Version < minOs)
            {
                scanButton.Text = string.Empty;
                scanButton.Image = Resources.IconZoom;
            }

            if (string.IsNullOrWhiteSpace(Settings.Default.QuarantinedFilesPath))
            {
                Settings.Default.QuarantinedFilesPath = settingsController.DefaultQuarantinedFilesPath;
                Settings.Default.Save();
            }
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

            UpdateLanguageComboBox();
        }

        private void GameLocationTextBoxTextChanged(object sender, EventArgs e)
        {
            if (gameLocationTextBox.Text.Length < 1)
            {
                gameLocationTextBox.Text = LocalizationStrings.SelectGameLocation;
            }

            gameLocationTextBox.ForeColor = gameLocationTextBox.Text.Equals(
                LocalizationStrings.SelectGameLocation, StringComparison.OrdinalIgnoreCase)
                ? Color.Gray
                : Color.Black;

            UpdateLanguageComboBox();
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

            if (!ValidateResolution())
            {
                Log.Info(string.Format("Invalid resolution: \"{0}\"", resolutionComboBox.Text.Trim()));
                MessageBox.Show(
                    this,
                    LocalizationStrings.ResolutionMustBeInTheFormatNumberXNumber,
                    LocalizationStrings.ValidationError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
                return;
            }

            if (backgroundImageListView.SelectedIndices.Count > 0)
            {
                Settings.Default.Wallpaper = backgroundImageListView.SelectedIndices[0] + 1;
            }

            if (renderModeComboBox.SelectedIndex > 0)
            {
                var renderMode = ((ComboBoxItem<GameArgumentsHelper.RenderMode>)renderModeComboBox.SelectedItem).Value;
                Settings.Default.LauncherRenderMode = renderMode.ToString();
            }
            else
            {
                Settings.Default.LauncherRenderMode = string.Empty;
            }

            var regex = new Regex(@"\d+x\d+");
            var resolution = resolutionComboBox.Text.Trim();
            Settings.Default.LauncherResolution = regex.IsMatch(resolution) ? resolution : string.Empty;

            Settings.Default.Launcher32BitColourDepth =
                ((ComboBoxItem<GameArgumentsHelper.ColorDepth>)colourDepthComboBox.SelectedItem).Value
                == GameArgumentsHelper.ColorDepth.Bits32;

            Settings.Default.LauncherCursorColour = cursorColourComboBox.SelectedIndex > 0
                                                        ? ((ComboBoxItem<GameArgumentsHelper.CursorColorDepth>)
                                                           cursorColourComboBox.SelectedItem).Value.ToString()
                                                        : string.Empty;

            int numCpus;
            int.TryParse(
                cpuCountComboBox.Text.Trim(), out numCpus);
            Settings.Default.LauncherCpuCount = numCpus;

            Settings.Default.LauncherCpuPriority = cpuPriorityComboBox.SelectedIndex > 0
                                                       ? ((ComboBoxItem<GameArgumentsHelper.CpuPriority>)
                                                          cpuPriorityComboBox.SelectedItem).Value.ToString()
                                                       : string.Empty;

            Settings.Default.Save();

            settingsController.UpdateMainFolder();

            Close();
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Log.Info("Closing settings form");

            Settings.Default.Reload();
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
                UpdateLanguageComboBox();
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

            UpdateRenderModeComboBox();

            UpdateColourDepthComboBox();

            UpdateCpuCountComboBox();

            UpdateCpuPriorityComboBox();

            UpdateCursorColourComboBox();

            UpdateLanguageComboBox();

            UpdateBackgroundsListView();

            UpdateLoginStatus();

            UpdateRemoteDatabaseAccessSettings();
        }

        private void UpdateRemoteDatabaseAccessSettings()
        {
            enableRemoteDatabaseConnectionCheckbox.Checked =
                Settings.Default.AllowDependencyCheck
                && Settings.Default.FetchInfoFromRemote;
        }

        private void UpdateCursorColourComboBox()
        {
            cursorColourComboBox.BeginUpdate();
            cursorColourComboBox.Items.Clear();
            cursorColourComboBox.Items.Add(LocalizationStrings.Ignore);
            cursorColourComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsHelper.CursorColorDepth>(
                    LocalizationStrings.Disabled, GameArgumentsHelper.CursorColorDepth.Disabled));
            cursorColourComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsHelper.CursorColorDepth>(
                    LocalizationStrings.SystemCursors, GameArgumentsHelper.CursorColorDepth.SystemCursors));
            cursorColourComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsHelper.CursorColorDepth>(
                    LocalizationStrings.BlackAndWhite, GameArgumentsHelper.CursorColorDepth.BlackAndWhite));
            cursorColourComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsHelper.CursorColorDepth>(
                    LocalizationStrings.Colors16, GameArgumentsHelper.CursorColorDepth.Colors16));
            cursorColourComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsHelper.CursorColorDepth>(
                    LocalizationStrings.Colors256, GameArgumentsHelper.CursorColorDepth.Colors256));
            cursorColourComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsHelper.CursorColorDepth>(
                    LocalizationStrings.FullColors, GameArgumentsHelper.CursorColorDepth.FullColors));

            GameArgumentsHelper.CursorColorDepth selectedCursor;
            Enum.TryParse(Settings.Default.LauncherCursorColour, true, out selectedCursor);

            switch (selectedCursor)
            {
                case GameArgumentsHelper.CursorColorDepth.Disabled:
                    cursorColourComboBox.SelectedIndex = 1;
                    break;
                case GameArgumentsHelper.CursorColorDepth.SystemCursors:
                    cursorColourComboBox.SelectedIndex = 2;
                    break;
                case GameArgumentsHelper.CursorColorDepth.BlackAndWhite:
                    cursorColourComboBox.SelectedIndex = 3;
                    break;
                case GameArgumentsHelper.CursorColorDepth.Colors16:
                    cursorColourComboBox.SelectedIndex = 4;
                    break;
                case GameArgumentsHelper.CursorColorDepth.Colors256:
                    cursorColourComboBox.SelectedIndex = 5;
                    break;
                case GameArgumentsHelper.CursorColorDepth.FullColors:
                    cursorColourComboBox.SelectedIndex = 6;
                    break;
                default:
                    cursorColourComboBox.SelectedIndex = 0;
                    break;
            }

            cursorColourComboBox.EndUpdate();
        }

        private void UpdateCpuPriorityComboBox()
        {
            cpuPriorityComboBox.BeginUpdate();
            cpuPriorityComboBox.Items.Clear();
            cpuPriorityComboBox.Items.Add(LocalizationStrings.Ignore);
            cpuPriorityComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsHelper.CpuPriority>(LocalizationStrings.Low, GameArgumentsHelper.CpuPriority.Low));
            cpuPriorityComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsHelper.CpuPriority>(
                    LocalizationStrings.Medium, GameArgumentsHelper.CpuPriority.Medium));
            cpuPriorityComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsHelper.CpuPriority>(
                    LocalizationStrings.High, GameArgumentsHelper.CpuPriority.High));

            GameArgumentsHelper.CpuPriority selectedPriority;
            Enum.TryParse(Settings.Default.LauncherCpuPriority, true, out selectedPriority);
            switch (selectedPriority)
            {
                case GameArgumentsHelper.CpuPriority.Low:
                    cpuPriorityComboBox.SelectedIndex = 1;
                    break;
                case GameArgumentsHelper.CpuPriority.Medium:
                    cpuPriorityComboBox.SelectedIndex = 2;
                    break;
                case GameArgumentsHelper.CpuPriority.High:
                    cpuPriorityComboBox.SelectedIndex = 3;
                    break;
                default:
                    cpuPriorityComboBox.SelectedIndex = 0;
                    break;
            }

            cpuPriorityComboBox.EndUpdate();
        }

        private void UpdateCpuCountComboBox()
        {
            cpuCountComboBox.BeginUpdate();
            cpuCountComboBox.Items.Clear();
            cpuCountComboBox.Items.Add(LocalizationStrings.Ignore);
            for (var i = 1; i <= Environment.ProcessorCount; i++)
            {
                cpuCountComboBox.Items.Add(i);
            }

            cpuCountComboBox.SelectedIndex = Settings.Default.LauncherCpuCount;
            cpuCountComboBox.EndUpdate();
        }

        private void UpdateColourDepthComboBox()
        {
            colourDepthComboBox.BeginUpdate();
            colourDepthComboBox.Items.Clear();
            colourDepthComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsHelper.ColorDepth>(
                    LocalizationStrings.Bits16, GameArgumentsHelper.ColorDepth.Bits16));
            colourDepthComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsHelper.ColorDepth>(
                    LocalizationStrings.Bits32, GameArgumentsHelper.ColorDepth.Bits32));

            colourDepthComboBox.SelectedIndex = Settings.Default.Launcher32BitColourDepth ? 1 : 0;
            colourDepthComboBox.EndUpdate();
        }

        private void UpdateRenderModeComboBox()
        {
            renderModeComboBox.BeginUpdate();
            renderModeComboBox.Items.Clear();
            renderModeComboBox.Items.Add(LocalizationStrings.Ignore);
            renderModeComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsHelper.RenderMode>(
                    LocalizationStrings.DirectX, GameArgumentsHelper.RenderMode.DirectX));
            renderModeComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsHelper.RenderMode>(
                    LocalizationStrings.OpenGL, GameArgumentsHelper.RenderMode.OpenGl));
            renderModeComboBox.Items.Add(
                new ComboBoxItem<GameArgumentsHelper.RenderMode>(
                    LocalizationStrings.Software, GameArgumentsHelper.RenderMode.Software));

            GameArgumentsHelper.RenderMode selectedRenderMode;
            Enum.TryParse(Settings.Default.LauncherRenderMode, true, out selectedRenderMode);
            switch (selectedRenderMode)
            {
                case GameArgumentsHelper.RenderMode.DirectX:
                    renderModeComboBox.SelectedIndex = 1;
                    break;
                case GameArgumentsHelper.RenderMode.OpenGl:
                    renderModeComboBox.SelectedIndex = 2;
                    break;
                case GameArgumentsHelper.RenderMode.Software:
                    renderModeComboBox.SelectedIndex = 3;
                    break;
                default:
                    renderModeComboBox.SelectedIndex = 0;
                    break;
            }

            renderModeComboBox.EndUpdate();
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

        private void UpdateLanguageComboBox()
        {
            if (!settingsController.ValidateGameLocationPath(Settings.Default.GameLocation))
            {
                return;
            }

            var languages = settingsController.GetInstalledLanguages();

            languageComboBox.BeginUpdate();
            languageComboBox.Items.Clear();
            languageComboBox.Items.Add(LocalizationStrings.Ignore);
            languageComboBox.Items.AddRange(languages.Cast<object>().ToArray());

            languageComboBox.SelectedItem = string.IsNullOrWhiteSpace(Settings.Default.LauncherLanguage)
                                                ? LocalizationStrings.Ignore
                                                : Settings.Default.LauncherLanguage;

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
                fetchInformationFromRemoteCheckbox.Checked = true;
                fetchInformationFromRemoteCheckbox.Enabled = true;
                allowCheckMissingDependenciesCheckBox.Checked = true;
                allowCheckMissingDependenciesCheckBox.Enabled = true;
            }
            else
            {
                fetchInformationFromRemoteCheckbox.Checked = false;
                fetchInformationFromRemoteCheckbox.Enabled = false;
                allowCheckMissingDependenciesCheckBox.Checked = false;
                allowCheckMissingDependenciesCheckBox.Enabled = false;
            }
        }

        private bool ValidateResolution()
        {
            var regEx = new Regex(@"\d+x\d+");
            var text = resolutionComboBox.Text.Trim();

            return regEx.IsMatch(text) || string.IsNullOrWhiteSpace(text)
                    || text.Equals(LocalizationStrings.Ignore, StringComparison.OrdinalIgnoreCase);
        }

        private void BrowseQuarantinedButtonClick(object sender, EventArgs e)
        {
            var result = storeLocationDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                var path = this.storeLocationDialog.SelectedPath;
                this.SetQuarantinedPath(path);
            }
        }

        private void QuarantinedFilesLocationTextBoxTextChanged(object sender, EventArgs e)
        {
            var path = quarantinedFilesLocationTextBox.Text.Trim();
            this.SetQuarantinedPath(path);
        }

        private void SetQuarantinedPath(string path)
        {
            Settings.Default.QuarantinedFilesPath = path;
            Log.Info(string.Format("Setting quarantined path to: {0}", path));
        }
    }
}
