namespace NIHEI.SC4Buddy.View.Application
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using Microsoft.Win32;

    using NIHEI.SC4Buddy.Control;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Properties;
    using NIHEI.SC4Buddy.View.UserFolders;

    public partial class SettingsForm : Form
    {
        private readonly UserFolderRegistry userFolderRegistry;

        public SettingsForm()
        {
            userFolderRegistry = RegistryFactory.UserFolderRegistry;

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

            if (!ValidateGameLocationPath(path))
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
            if (!ValidateGameLocationPath(gameLocationTextBox.Text))
            {
                return;
            }

            if (backgroundImageListView.SelectedIndices.Count > 0)
            {
                Settings.Default.Wallpaper = backgroundImageListView.SelectedIndices[0] + 1;
            }

            Settings.Default.Save();

            UpdateMainFolder();

            Close();
        }

        private bool ValidateGameLocationPath(string path, bool showErrors = true)
        {
            if (File.Exists(Path.Combine(path, @"Apps\SimCity 4.exe")))
            {
                return true;
            }

            if (showErrors)
            {
                errorProvider.SetIconPadding(gameLocationTextBox, ManageUserFoldersForm.ErrorIconPadding);
                errorProvider.SetError(gameLocationTextBox, LocalizationStrings.InvalidGameLocationFolder);
            }

            return false;
        }

        private void UpdateMainFolder()
        {
            var folder = userFolderRegistry.UserFolders.FirstOrDefault(x => x.Id == 1);
            if (folder == null)
            {
                throw new InvalidOperationException("Main plugin folder has been deleted from the database.");
            }

            folder.Path = Path.Combine(Settings.Default.GameLocation, "Plugins");
            userFolderRegistry.Update(folder);
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void ScanButtonClick(object sender, EventArgs e)
        {
            const string RegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

            using (var key = Registry.LocalMachine.OpenSubKey(RegistryKey))
            {
                if (key == null)
                {
                    return;
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

                        gameLocationTextBox.Text = path;
                        break;
                    }
                }
            }

            UpdateLanguageBox();
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

            if (SessionController.Instance.IsLoggedIn)
            {
                logoutButton.Enabled = true;
                loginButton.Enabled = false;
                requestLoginButton.Enabled = false;
                loginStatusLabel.Text = string.Format(
                    LocalizationStrings.LoggedInAs, SessionController.Instance.User.Username);
            }
            else
            {
                loginButton.Enabled = true;
                requestLoginButton.Enabled = true;
                logoutButton.Enabled = false;
                loginStatusLabel.Text = LocalizationStrings.NoUserIsLoggedIn;
            }
        }

        private void UpdateLanguageBox()
        {
            if (!ValidateGameLocationPath(Settings.Default.GameLocation, false))
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
            Console.WriteLine(e.CloseReason);

            Settings.Default.Reload();

            if (ValidateGameLocationPath(gameLocationTextBox.Text))
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

            if (result == DialogResult.Retry)
            {
                e.Cancel = true;
            }
        }
    }
}
