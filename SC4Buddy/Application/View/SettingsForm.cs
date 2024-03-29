﻿namespace Nihei.SC4Buddy.Application.View
{
    using System;
    using System.Configuration;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using log4net;
    using Nihei.Common.UI.Elements;
    using Nihei.SC4Buddy.Application.Control;
    using Nihei.SC4Buddy.Application.Models;
    using Nihei.SC4Buddy.Configuration;
    using Nihei.SC4Buddy.Properties;
    using Nihei.SC4Buddy.Resources;
    using Nihei.SC4Buddy.UserFolders.Control;
    using ColorDepth = Nihei.SC4Buddy.Application.Models.ColorDepth;

    public partial class SettingsForm : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Version MinOsVersion = new Version(6, 2);

        private readonly ISettingsController settingsController;

        public SettingsForm(IUserFoldersController userFoldersController)
        {
            InitializeComponent();

            settingsController = new SettingsController(userFoldersController);

            if (Environment.OSVersion.Version < MinOsVersion)
            {
                scanButton.Text = string.Empty;
                scanButton.Image = Resources.IconZoom;
            }

            if (!Settings.HasSetting(Settings.Keys.QuarantinedFiles))
            {
                Settings.SetAndSave(
                    Settings.Keys.QuarantinedFiles,
                    Path.Combine(Settings.GetDefaultStorageLocation(), "QuarantinedFiles"));
            }
        }

        private void AutoSaveIntervalTrackBarScroll(object sender, EventArgs e)
        {
            var interval = autoSaveIntervalTrackBar.Value;

            UpdateAutoSaveLabel(interval);
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
                Log.Info($"Browsed to valid game location: {path}");
                gameLocationTextBox.Text = path;
            }

            UpdateLanguageComboBox();
        }

        private void BrowseQuarantinedButtonClick(object sender, EventArgs e)
        {
            var result = storeLocationDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                quarantinedFilesLocationTextBox.Text = storeLocationDialog.SelectedPath;
            }
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Log.Info("Closing settings form");
            Hide();
        }

        private void DisableAudioCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            disableMusicCheckBox.Enabled = !disableAudioCheckBox.Checked;
            disableSoundsCheckBox.Enabled = !disableAudioCheckBox.Checked;
        }

        private void EnableAutoSaveButtonCheckedChanged(object sender, EventArgs e)
        {
            autoSaveIntervalTrackBar.Enabled = enableAutoSaveCheckBox.Checked;

            AutoSaveIntervalTrackBarScroll(sender, e);
        }

        private void GameLocationTextBoxTextChanged(object sender, EventArgs e)
        {
            if (gameLocationTextBox.Text.Length < 1)
            {
                gameLocationTextBox.Text = LocalizationStrings.SelectGameLocation;
            }

            gameLocationTextBox.ForeColor = gameLocationTextBox.Text.Equals(
                                                LocalizationStrings.SelectGameLocation,
                                                StringComparison.OrdinalIgnoreCase)
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
            }

            Settings.SetAndSave(Settings.Keys.GameLocation, gameLocationTextBox.Text);

            if (!ValidateResolution())
            {
                Log.Info($"Invalid resolution: \"{resolutionComboBox.Text.Trim()}\"");
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
                Settings.SetAndSave(Settings.Keys.Wallpaper, backgroundImageListView.SelectedIndices[0] + 1);
            }

            if (renderModeComboBox.SelectedIndex > 0)
            {
                var renderMode = ((ComboBoxItem<RenderMode>)renderModeComboBox.SelectedItem).Value;
                LauncherSettings.SetAndSave(LauncherSettings.Keys.RenderMode, renderMode.ToString());
            }
            else
            {
                LauncherSettings.SetAndSave(LauncherSettings.Keys.RenderMode, string.Empty);
            }

            var regex = new Regex(@"\d+x\d+");
            var resolution = resolutionComboBox.Text.Trim();
            LauncherSettings.SetAndSave(
                LauncherSettings.Keys.Resolution,
                regex.IsMatch(resolution) ? resolution : string.Empty);

            var colorDepth = ((ComboBoxItem<ColorDepth>)colourDepthComboBox.SelectedItem).Value;
            LauncherSettings.SetAndSave(
                LauncherSettings.Keys.ColourDepth32Bit,
                colorDepth == ColorDepth.Bits32);

            var cursorColour = cursorColourComboBox.SelectedIndex > 0 ?
                ((ComboBoxItem<CursorColorDepth>)cursorColourComboBox.SelectedItem).Value.ToString()
                : string.Empty;
            LauncherSettings.SetAndSave(
                LauncherSettings.Keys.CursorColourDepth,
                cursorColour);

            var numCpus = Convert.ToInt32(cpuCountComboBox.Text.Trim());
            LauncherSettings.SetAndSave(LauncherSettings.Keys.CpuCount, numCpus);

            var cpuPriority = cpuPriorityComboBox.SelectedIndex > 0
                                  ? ((ComboBoxItem<CpuPriority>)
                                        cpuPriorityComboBox.SelectedItem).Value.ToString()
                                  : string.Empty;

            LauncherSettings.SetAndSave(LauncherSettings.Keys.CpuPriority, cpuPriority);

            settingsController.CheckMainFolder();

            LauncherSettings.SetAndSave(LauncherSettings.Keys.EnableAutoSave, enableAutoSaveCheckBox.Checked);
            LauncherSettings.SetAndSave(LauncherSettings.Keys.AutoSaveWaitTime, autoSaveIntervalTrackBar.Value);
            LauncherSettings.SetAndSave(LauncherSettings.Keys.DisableAudio, disableAudioCheckBox.Checked);
            LauncherSettings.SetAndSave(LauncherSettings.Keys.DisableMusic, disableMusicCheckBox.Checked);
            LauncherSettings.SetAndSave(LauncherSettings.Keys.DisableSounds, disableSoundsCheckBox.Checked);

            LauncherSettings.SetAndSave(LauncherSettings.Keys.EnableCustomResolution, customResolutionCheckBox.Checked);
            LauncherSettings.SetAndSave(LauncherSettings.Keys.WindowMode, windowModeCheckBox.Checked);

            LauncherSettings.SetAndSave(LauncherSettings.Keys.SkipIntro, skipIntroCheckBox.Checked);
            LauncherSettings.SetAndSave(LauncherSettings.Keys.PauseWhenMinimized, pauseMinimizedCheckBox.Checked);
            LauncherSettings.SetAndSave(
                LauncherSettings.Keys.DisableExceptionHandling,
                disableExceptionHandlingCheckBox.Checked);
            LauncherSettings.SetAndSave(
                LauncherSettings.Keys.DisableBackgroundLoader,
                disableBackgroundLoaderCheckBox.Checked);

            LauncherSettings.SetAndSave(LauncherSettings.Keys.Language, languageComboBox.SelectedItem);
            LauncherSettings.SetAndSave(LauncherSettings.Keys.IgnoreMissingModels, ignoreMissingModelsCheckBox.Checked);
            LauncherSettings.SetAndSave(LauncherSettings.Keys.DisableIme, disableIMECheckBox.Checked);
            LauncherSettings.SetAndSave(LauncherSettings.Keys.WriteLog, writeLogCheckBox.Checked);

            Settings.SetAndSave(
                Settings.Keys.AllowCheckForMissingDependencies,
                allowCheckMissingDependenciesCheckBox.Checked);
            Settings.SetAndSave(
                Settings.Keys.AskForAdditionalInformationAfterInstallation,
                AskForAdditionalInfoAfterInstallCheckBox.Checked);
            Settings.SetAndSave(
                Settings.Keys.AskToRemoveNonPluginFilesAfterInstallation,
                RemoveNonPluginFilesAfterInstallCheckBox.Checked);
            Settings.SetAndSave(
                Settings.Keys.AutoRunExecutablesDuringInstallation,
                AutoRunInstallerExecutablesCheckBox.Checked);

            Settings.SetAndSave(Settings.Keys.ApiBaseUrl, apiBaseUrlTextBox.Text.Trim());
            Settings.SetAndSave(
                Settings.Keys.FetchInformationFromRemoteServer,
                fetchInformationFromRemoteCheckbox.Checked);
            Settings.SetAndSave(Settings.Keys.DetectPlugins, detectPluginsCheckBox.Checked);

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

        private void SettingsFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (settingsController.ValidateGameLocationPath(Settings.Get(Settings.Keys.GameLocation)))
            {
                return;
            }

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

        private void SettingsFormLoad(object sender, EventArgs e)
        {
            gameLocationTextBox.Text = Settings.Get(Settings.Keys.GameLocation);

            if (string.IsNullOrWhiteSpace(gameLocationTextBox.Text))
            {
                gameLocationTextBox.Text = LocalizationStrings.SelectGameLocation;
                gameLocationTextBox.ForeColor = Color.Gray;
            }

            gameLocationTextBox.ForeColor = gameLocationTextBox.Text.Equals(
                                                LocalizationStrings.SelectGameLocation,
                                                StringComparison.OrdinalIgnoreCase)
                                                ? Color.Gray
                                                : Color.Black;

            enableAutoSaveCheckBox.Checked = LauncherSettings.Get<bool>(LauncherSettings.Keys.EnableAutoSave);
            autoSaveIntervalTrackBar.Enabled = LauncherSettings.Get<bool>(LauncherSettings.Keys.EnableAutoSave);
            UpdateAutoSaveLabel(LauncherSettings.GetInt(LauncherSettings.Keys.AutoSaveWaitTime));

            disableAudioCheckBox.Checked = LauncherSettings.Get<bool>(LauncherSettings.Keys.DisableAudio);
            disableMusicCheckBox.Checked = LauncherSettings.Get<bool>(LauncherSettings.Keys.DisableMusic);
            disableSoundsCheckBox.Checked = LauncherSettings.Get<bool>(LauncherSettings.Keys.DisableSounds);

            customResolutionCheckBox.Checked = LauncherSettings.Get<bool>(LauncherSettings.Keys.EnableCustomResolution);
            windowModeCheckBox.Checked = LauncherSettings.Get<bool>(LauncherSettings.Keys.WindowMode);

            UpdateResolutionComboBox();
            UpdateRenderModeComboBox();
            UpdateColourDepthComboBox();
            UpdateCursorColourComboBox();

            UpdateCpuCountComboBox();
            UpdateCpuPriorityComboBox();

            skipIntroCheckBox.Checked = LauncherSettings.Get<bool>(LauncherSettings.Keys.SkipIntro);
            pauseMinimizedCheckBox.Checked = LauncherSettings.Get<bool>(LauncherSettings.Keys.PauseWhenMinimized);
            disableExceptionHandlingCheckBox.Checked =
                LauncherSettings.Get<bool>(LauncherSettings.Keys.DisableExceptionHandling);
            disableBackgroundLoaderCheckBox.Checked =
                LauncherSettings.Get<bool>(LauncherSettings.Keys.DisableBackgroundLoader);

            UpdateLanguageComboBox();

            ignoreMissingModelsCheckBox.Checked = LauncherSettings.Get<bool>(LauncherSettings.Keys.IgnoreMissingModels);
            disableIMECheckBox.Checked = LauncherSettings.Get<bool>(LauncherSettings.Keys.DisableIme);
            writeLogCheckBox.Checked = LauncherSettings.Get<bool>(LauncherSettings.Keys.WriteLog);

            UpdateBackgroundsListView();

            quarantinedFilesLocationTextBox.Text = Settings.Get(Settings.Keys.QuarantinedFiles);

            allowCheckMissingDependenciesCheckBox.Checked =
                Settings.Get<bool>(Settings.Keys.AllowCheckForMissingDependencies);
            AskForAdditionalInfoAfterInstallCheckBox.Checked =
                Settings.Get<bool>(Settings.Keys.AskForAdditionalInformationAfterInstallation);
            RemoveNonPluginFilesAfterInstallCheckBox.Checked =
                Settings.Get<bool>(Settings.Keys.AskToRemoveNonPluginFilesAfterInstallation);
            AutoRunInstallerExecutablesCheckBox.Checked =
                Settings.Get<bool>(Settings.Keys.AutoRunExecutablesDuringInstallation);

            apiBaseUrlTextBox.Text = Settings.Get(
                Settings.Keys.ApiBaseUrl,
                ConfigurationManager.AppSettings["ApiBaseUrl"]);
            detectPluginsCheckBox.Checked = Settings.Get(Settings.Keys.DetectPlugins, true);
            fetchInformationFromRemoteCheckbox.Checked =
                Settings.Get<bool>(Settings.Keys.FetchInformationFromRemoteServer);
        }

        private void UpdateAutoSaveLabel(int interval)
        {
            autoSaveIntervalLabel.Text = string.Format(LocalizationStrings.NumMinutes, interval);
            shortAutosaveIntervalsLabel.Visible = interval < 10;
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

        private void UpdateColourDepthComboBox()
        {
            colourDepthComboBox.BeginUpdate();
            colourDepthComboBox.Items.Clear();
            colourDepthComboBox.Items.Add(
                new ComboBoxItem<ColorDepth>(
                    LocalizationStrings.Bits16,
                    ColorDepth.Bits16));
            colourDepthComboBox.Items.Add(
                new ComboBoxItem<ColorDepth>(
                    LocalizationStrings.Bits32,
                    ColorDepth.Bits32));

            colourDepthComboBox.SelectedIndex =
                LauncherSettings.Get<bool>(LauncherSettings.Keys.ColourDepth32Bit) ? 1 : 0;
            colourDepthComboBox.EndUpdate();
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

            cpuCountComboBox.SelectedIndex = LauncherSettings.GetInt(LauncherSettings.Keys.CpuCount);
            cpuCountComboBox.EndUpdate();
        }

        private void UpdateCpuPriorityComboBox()
        {
            cpuPriorityComboBox.BeginUpdate();
            cpuPriorityComboBox.Items.Clear();
            cpuPriorityComboBox.Items.Add(LocalizationStrings.Ignore);
            cpuPriorityComboBox.Items.Add(
                new ComboBoxItem<CpuPriority>(
                    LocalizationStrings.Low,
                    CpuPriority.Low));
            cpuPriorityComboBox.Items.Add(
                new ComboBoxItem<CpuPriority>(
                    LocalizationStrings.Medium,
                    CpuPriority.Medium));
            cpuPriorityComboBox.Items.Add(
                new ComboBoxItem<CpuPriority>(
                    LocalizationStrings.High,
                    CpuPriority.High));

            Enum.TryParse(LauncherSettings.Get(LauncherSettings.Keys.CpuPriority), true, out CpuPriority selectedPriority);
            switch (selectedPriority)
            {
                case CpuPriority.Low:
                    cpuPriorityComboBox.SelectedIndex = 1;
                    break;
                case CpuPriority.Medium:
                    cpuPriorityComboBox.SelectedIndex = 2;
                    break;
                case CpuPriority.High:
                    cpuPriorityComboBox.SelectedIndex = 3;
                    break;
                default:
                    cpuPriorityComboBox.SelectedIndex = 0;
                    break;
            }

            cpuPriorityComboBox.EndUpdate();
        }

        private void UpdateCursorColourComboBox()
        {
            cursorColourComboBox.BeginUpdate();
            cursorColourComboBox.Items.Clear();
            cursorColourComboBox.Items.Add(LocalizationStrings.Ignore);
            cursorColourComboBox.Items.Add(
                new ComboBoxItem<CursorColorDepth>(
                    LocalizationStrings.Disabled,
                    CursorColorDepth.Disabled));
            cursorColourComboBox.Items.Add(
                new ComboBoxItem<CursorColorDepth>(
                    LocalizationStrings.SystemCursors,
                    CursorColorDepth.SystemCursors));
            cursorColourComboBox.Items.Add(
                new ComboBoxItem<CursorColorDepth>(
                    LocalizationStrings.BlackAndWhite,
                    CursorColorDepth.BlackAndWhite));
            cursorColourComboBox.Items.Add(
                new ComboBoxItem<CursorColorDepth>(
                    LocalizationStrings.Colors16,
                    CursorColorDepth.Colors16));
            cursorColourComboBox.Items.Add(
                new ComboBoxItem<CursorColorDepth>(
                    LocalizationStrings.Colors256,
                    CursorColorDepth.Colors256));
            cursorColourComboBox.Items.Add(
                new ComboBoxItem<CursorColorDepth>(
                    LocalizationStrings.FullColors,
                    CursorColorDepth.FullColors));

            Enum.TryParse(LauncherSettings.Get(LauncherSettings.Keys.CursorColourDepth), true, out CursorColorDepth selectedCursor);

            switch (selectedCursor)
            {
                case CursorColorDepth.Disabled:
                    cursorColourComboBox.SelectedIndex = 1;
                    break;
                case CursorColorDepth.SystemCursors:
                    cursorColourComboBox.SelectedIndex = 2;
                    break;
                case CursorColorDepth.BlackAndWhite:
                    cursorColourComboBox.SelectedIndex = 3;
                    break;
                case CursorColorDepth.Colors16:
                    cursorColourComboBox.SelectedIndex = 4;
                    break;
                case CursorColorDepth.Colors256:
                    cursorColourComboBox.SelectedIndex = 5;
                    break;
                case CursorColorDepth.FullColors:
                    cursorColourComboBox.SelectedIndex = 6;
                    break;
                default:
                    cursorColourComboBox.SelectedIndex = 0;
                    break;
            }

            cursorColourComboBox.EndUpdate();
        }

        private void UpdateLanguageComboBox()
        {
            if (!settingsController.ValidateGameLocationPath(Settings.Get(Settings.Keys.GameLocation)))
            {
                return;
            }

            var languages = settingsController.GetInstalledLanguages();

            languageComboBox.BeginUpdate();
            languageComboBox.Items.Clear();
            languageComboBox.Items.Add(LocalizationStrings.Ignore);
            languageComboBox.Items.AddRange(languages.Cast<object>().ToArray());

            languageComboBox.SelectedItem =
                string.IsNullOrWhiteSpace(LauncherSettings.Get(LauncherSettings.Keys.Language))
                    ? LocalizationStrings.Ignore
                    : LauncherSettings.Get(LauncherSettings.Keys.Language);

            languageComboBox.EndUpdate();
        }

        private void UpdateRenderModeComboBox()
        {
            renderModeComboBox.BeginUpdate();
            renderModeComboBox.Items.Clear();
            renderModeComboBox.Items.Add(LocalizationStrings.Ignore);
            renderModeComboBox.Items.Add(
                new ComboBoxItem<RenderMode>(
                    LocalizationStrings.DirectX,
                    RenderMode.DirectX));
            renderModeComboBox.Items.Add(
                new ComboBoxItem<RenderMode>(
                    LocalizationStrings.OpenGL,
                    RenderMode.OpenGl));
            renderModeComboBox.Items.Add(
                new ComboBoxItem<RenderMode>(
                    LocalizationStrings.Software,
                    RenderMode.Software));

            Enum.TryParse(LauncherSettings.Get(LauncherSettings.Keys.RenderMode), true, out RenderMode selectedRenderMode);

            switch (selectedRenderMode)
            {
                case RenderMode.DirectX:
                    renderModeComboBox.SelectedIndex = 1;

                    break;
                case RenderMode.OpenGl:
                    renderModeComboBox.SelectedIndex = 2;

                    break;
                case RenderMode.Software:
                    renderModeComboBox.SelectedIndex = 3;

                    break;
                default:
                    renderModeComboBox.SelectedIndex = 0;

                    break;
            }

            renderModeComboBox.EndUpdate();
        }

        private void UpdateResolutionComboBox()
        {
            resolutionComboBox.Text = LauncherSettings.Get(LauncherSettings.Keys.Resolution);
        }

        private bool ValidateResolution()
        {
            var regEx = new Regex(@"\d+x\d+");
            var text = resolutionComboBox.Text.Trim();

            return regEx.IsMatch(text)
                   || string.IsNullOrWhiteSpace(text)
                   || text.Equals(LocalizationStrings.Ignore, StringComparison.OrdinalIgnoreCase);
        }
    }
}
