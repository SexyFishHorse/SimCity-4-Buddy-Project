namespace NIHEI.SC4Buddy.View.Application
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Threading;
    using System.Windows.Forms;

    using log4net;
    using NIHEI.SC4Buddy.Configuration;
    using NIHEI.SC4Buddy.Control;
    using NIHEI.SC4Buddy.Control.Plugins;
    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Properties;
    using NIHEI.SC4Buddy.Remote;
    using NIHEI.SC4Buddy.View.Elements;
    using NIHEI.SC4Buddy.View.UserFolders;

    public partial class Sc4Buddy : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ResourceManager localizationManager;

        private readonly UserFolderController userFolderController;

        private readonly PluginController pluginController;

        private readonly PluginGroupController pluginGroupController;

        private readonly IPluginMatcher pluginMatcher;

        private readonly IDependencyChecker dependencyChecker;

        public Sc4Buddy(
            UserFolderController userFolderController,
            PluginController pluginController,
            PluginGroupController pluginGroupController,
            IPluginMatcher pluginMatcher,
            IDependencyChecker dependencyChecker)
        {
            this.userFolderController = userFolderController;
            this.pluginGroupController = pluginGroupController;
            this.pluginMatcher = pluginMatcher;
            this.dependencyChecker = dependencyChecker;
            this.pluginController = pluginController;

            InitializeComponent();

            localizationManager = new System.ComponentModel.ComponentResourceManager(typeof(Sc4Buddy));
        }

        private void UserFolderComboBoxCheckSelectedValue(object sender, EventArgs e)
        {
            var selectUserFolderText = localizationManager.GetString("userFolderComboBox.Text");
            if (userFolderComboBox.SelectedItem == null
                || userFolderComboBox.Text.Equals(selectUserFolderText))
            {
                userFolderComboBox.Text = selectUserFolderText;
                userFolderComboBox.ForeColor = Color.Gray;
            }
            else
            {
                userFolderComboBox.ForeColor = Color.Black;
            }
        }

        private void UserFolderComboBoxDropDown(object sender, EventArgs e)
        {
            userFolderComboBox.ForeColor = Color.Black;
        }

        private void ManageFoldersToolStripMenuItemClick(object sender, EventArgs e)
        {
            new ManageUserFoldersForm().ShowDialog(this);

            RepopulateUserFolderRelatives();
        }

        private void RepopulateUserFolderRelatives()
        {
            Log.Info("Repopulating user folder lists");

            userFolderComboBox.BeginUpdate();
            userFolderComboBox.Items.Clear();

            var remove = userFoldersToolStripMenuItem.DropDownItems.OfType<UserFolderToolStripMenuItem>().ToList();
            foreach (var item in remove)
            {
                userFoldersToolStripMenuItem.DropDownItems.Remove(item);
            }

            var insertIndex = 0;
            var comboboxIndex = 0;
            var startupFolderIndex = -1;
            foreach (var userFolder in userFolderController.UserFolders)
            {
                if (!userFolder.IsMainFolder)
                {
                    userFolderComboBox.Items.Add(new ComboBoxItem<UserFolder>(userFolder.Alias, userFolder));

                    if (userFolder.IsStartupFolder)
                    {
                        startupFolderIndex = comboboxIndex;
                    }

                    comboboxIndex++;
                }

                if (userFolder.Alias.Equals("?"))
                {
                    userFolder.Alias = LocalizationStrings.GameUserFolderName;
                    userFolderController.Update(userFolder);
                }

                userFoldersToolStripMenuItem.DropDownItems.Insert(insertIndex, new UserFolderToolStripMenuItem(userFolder, UserFolderMenuItemClick));
                insertIndex++;
            }

            if (startupFolderIndex >= 0)
            {
                userFolderComboBox.SelectedIndex = startupFolderIndex;
            }
            else
            {
                userFolderComboBox.SelectedItem = null;
                userFolderComboBox.Text = localizationManager.GetString("userFolderComboBox.Text");
                userFolderComboBox.ForeColor = Color.Gray;
            }

            userFolderComboBox.EndUpdate();
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Log.Info("Application closing (menu item click)");
            Close();
        }

        private void Sc4BuddyLoad(object sender, EventArgs e)
        {
            RepopulateUserFolderRelatives();

            UpdateBackground();
        }

        private void UpdateBackground()
        {
            Bitmap image;
            switch (Settings.GetInt(Settings.Keys.Wallpaper))
            {
                case 13:
                    image = Resources.Wallpaper13;
                    break;
                case 12:
                    image = Resources.Wallpaper12;
                    break;
                case 11:
                    image = Resources.Wallpaper11;
                    break;
                case 10:
                    image = Resources.Wallpaper10;
                    break;
                case 9:
                    image = Resources.Wallpaper9;
                    break;
                case 8:
                    image = Resources.Wallpaper8;
                    break;
                case 7:
                    image = Resources.Wallpaper7;
                    break;
                case 6:
                    image = Resources.Wallpaper6;
                    break;
                case 5:
                    image = Resources.Wallpaper5;
                    break;
                case 4:
                    image = Resources.Wallpaper4;
                    break;
                case 3:
                    image = Resources.Wallpaper3;
                    break;
                case 2:
                    image = Resources.Wallpaper2;
                    break;
                default:
                    image = Resources.Wallpaper1;
                    break;
            }

            backgroundPanel.BackgroundImage = image;
        }

        private void UserFolderMenuItemClick(object sender, EventArgs e)
        {
            new UserFolderForm(
                pluginController,
                pluginGroupController,
                userFolderController,
                ((UserFolderToolStripMenuItem)sender).UserFolder,
                pluginMatcher, dependencyChecker).ShowDialog(this);
        }

        private void PlayButtonClick(object sender, EventArgs e)
        {
            Log.Info("Launching game");
            playButton.Enabled = false;
            playButton.Text = LocalizationStrings.StartingGame;
            playButton.ForeColor = Color.Gray;
            playButton.Update();

            UserFolder selectedUserFolder = null;
            if (userFolderComboBox.SelectedItem != null)
            {
                selectedUserFolder = ((ComboBoxItem<UserFolder>)userFolderComboBox.SelectedItem).Value;
                Log.Info(
                    string.Format(
                        "Selected user folder is {0} (id: {1})",
                        selectedUserFolder.Alias,
                        selectedUserFolder.Id));
            }
            else
            {
                Log.Info("No user folder selected.");
            }

            var arguments = new GameArgumentsHelper().GetArgumentString(selectedUserFolder);

            var gameProcessStartInfo = new ProcessStartInfo
                                           {
                                               FileName =
                                                   Path.Combine(
                                                       Settings.Get(Settings.Keys.GameLocation),
                                                       "Apps",
                                                       "SimCity 4.exe"),
                                               Arguments = arguments,
                                               WorkingDirectory = Settings.Get(Settings.Keys.GameLocation)
                                           };

            var gameLauncher = new GameLauncher(gameProcessStartInfo, LauncherSettings.GetInt(LauncherSettings.Keys.AutoSaveWaitTime));
            var gameLauncherThread = new Thread(gameLauncher.Start) { Name = "SC4Buddy AutoSaver" };

            gameLauncherThread.Start();

            Thread.Sleep(5000);

            playButton.Enabled = true;
            playButton.Text = localizationManager.GetString("playButton.Text");
            playButton.ForeColor = Color.Black;
        }

        private void SettingsToolStripMenuItemClick(object sender, EventArgs e)
        {
            new SettingsForm(userFolderController).ShowDialog(this);

            UpdateBackground();
        }

        private void SupportToolStripMenuItemClick(object sender, EventArgs e)
        {
            Process.Start("http://community.simtropolis.com/topic/58814-the-simcity-4-buddy-project/");
        }

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

        private void BugsAndFeedbackToolStripMenuItemClick(object sender, EventArgs e)
        {
            Process.Start("https://github.com/NIHEI-Systems/sc4buddy/issues");
        }

        private void OpenLogFileToolStripMenuItemClick(object sender, EventArgs e)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Irradiated Games", "SimCity 4 Buddy", "Logs");

            var file = string.Format("log-{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"));

            var filePath = Path.Combine(path, file);

            if (!File.Exists(filePath))
            {
                MessageBox.Show(
                    this,
                    string.Format("No log file was found. Check the folder ({0}) manually.", path),
                    "No logfile was found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            Process.Start(filePath);
        }
    }
}
