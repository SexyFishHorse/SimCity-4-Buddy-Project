namespace NIHEI.SC4Buddy.View.Application
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using System.Resources;
    using System.Threading;
    using System.Windows.Forms;

    using log4net;

    using NIHEI.SC4Buddy.Control;
    using NIHEI.SC4Buddy.Control.Plugins;
    using NIHEI.SC4Buddy.Control.Remote;
    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.Entities;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Properties;
    using NIHEI.SC4Buddy.View.Author;
    using NIHEI.SC4Buddy.View.Developer;
    using NIHEI.SC4Buddy.View.Elements;
    using NIHEI.SC4Buddy.View.UserFolders;

    public partial class Sc4Buddy : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ResourceManager localizationManager;

        private readonly UserFolderController userFolderController;

        private readonly PluginController pluginController;

        private readonly PluginGroupController pluginGroupController;

        private readonly RemotePluginController remotePluginController;

        private readonly RemotePluginFileController remotePluginFileController;

        private readonly AuthorController authorController;

        public Sc4Buddy(
            UserFolderController userFolderController,
            PluginController pluginController,
            PluginGroupController pluginGroupController,
            RemotePluginController remotePluginController,
            RemotePluginFileController remotePluginFileController,
            AuthorController authorController)
        {
            this.userFolderController = userFolderController;
            this.remotePluginController = remotePluginController;
            this.authorController = authorController;
            this.remotePluginFileController = remotePluginFileController;
            this.pluginGroupController = pluginGroupController;
            this.pluginController = pluginController;

            InitializeComponent();

            localizationManager = new System.ComponentModel.ComponentResourceManager(typeof(Sc4Buddy));

            SessionController.Instance.UserLoggedIn += OnUserLoggedIn;
            SessionController.Instance.UserLoggedOut += OnUserLoggedOut;
        }

        private void OnUserLoggedOut(SessionController sender, SessionEventArgs eventargs)
        {
            UpdateToolsVisibility();
        }

        private void OnUserLoggedIn(SessionController sender, SessionEventArgs eventArgs)
        {
            UpdateToolsVisibility();
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
            userFolderComboBox.BeginUpdate();
            userFolderComboBox.Items.Clear();

            var remove = userFoldersToolStripMenuItem.DropDownItems.OfType<UserFolderToolStripMenuItem>().ToList();
            foreach (var item in remove)
            {
                userFoldersToolStripMenuItem.DropDownItems.Remove(item);
            }

            var insertIndex = 0;
            foreach (var userFolder in userFolderController.UserFolders)
            {
                if (userFolder.Id != 1)
                {
                    userFolderComboBox.Items.Add(new ComboBoxItem<UserFolder>(userFolder.Alias, userFolder));
                }

                if (userFolder.Alias.Equals("?"))
                {
                    userFolder.Alias = LocalizationStrings.GameUserFolderName;
                    userFolderController.Update(userFolder);
                }

                userFoldersToolStripMenuItem.DropDownItems.Insert(insertIndex, new UserFolderToolStripMenuItem(userFolder, UserFolderMenuItemClick));
                insertIndex++;
            }

            userFolderComboBox.EndUpdate();
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
        }

        private void Sc4BuddyLoad(object sender, EventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                AttemptAutoLogin();
            }

            UpdateToolsVisibility();

            RepopulateUserFolderRelatives();

            UpdateBackground();
        }

        private void AttemptAutoLogin()
        {
            try
            {
                SessionController.Instance.AttemptAutoLogin();
            }
            catch (Exception ex)
            {
                Log.Error("Error during auto-login", ex);

                if (MessageBox.Show(
                    this,
                    LocalizationStrings.UnableToLogYouIntoTheSystem,
                    LocalizationStrings.ErrorOccuredDuringLogin,
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Error) == DialogResult.Retry)
                {
                    AttemptAutoLogin();
                }
            }
        }

        private void UpdateToolsVisibility()
        {
            developerToolStripMenuItem.Visible = false;
            addPluginInformationToolStripMenuItem.Visible = false;
            updatePluginInformationToolStripMenuItem.Visible = false;
            myAuthorsToolStripMenuItem.Visible = false;

            if (!SessionController.Instance.IsLoggedIn)
            {
                toolsToolStripMenuItem.Visible = false;
                return;
            }

            toolsToolStripMenuItem.Visible = true;

            if (SessionController.Instance.User.IsDeveloper)
            {
                developerToolStripMenuItem.Visible = true;
            }

            if (SessionController.Instance.User.IsAuthor || SessionController.Instance.User.IsDeveloper)
            {
                addPluginInformationToolStripMenuItem.Visible = true;
                updatePluginInformationToolStripMenuItem.Visible = true;
                myAuthorsToolStripMenuItem.Visible = true;
            }
        }

        private void UpdateBackground()
        {
            Bitmap image;
            switch (Settings.Default.Wallpaper)
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
                ((UserFolderToolStripMenuItem)sender).UserFolder).ShowDialog(this);
        }

        private void PlayButtonClick(object sender, EventArgs e)
        {
            playButton.Enabled = false;
            playButton.Text = LocalizationStrings.StartingGame;
            playButton.ForeColor = Color.Gray;
            playButton.Update();

            UserFolder selectedUserFolder = null;
            if (userFolderComboBox.SelectedItem != null)
            {
                selectedUserFolder = ((ComboBoxItem<UserFolder>)userFolderComboBox.SelectedItem).Value;
            }

            var arguments = new GameArgumentsHelper().GetArgumentString(selectedUserFolder);

            var gameProcessStartInfo = new ProcessStartInfo
                                           {
                                               FileName =
                                                   Path.Combine(
                                                       Settings.Default.GameLocation,
                                                       "Apps",
                                                       "SimCity 4.exe"),
                                               Arguments = arguments
                                           };

            var gameLauncher = new GameLauncher(gameProcessStartInfo, Settings.Default.AutoSaveWaitTime);
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

        private void DeveloperToolStripMenuItemClick(object sender, EventArgs e)
        {
            new DeveloperForm(
                pluginController,
                pluginGroupController,
                userFolderController,
                authorController,
                remotePluginController,
                remotePluginFileController).ShowDialog(this);
        }

        private void MyAuthorsToolStripMenuItemClick(object sender, EventArgs e)
        {
            new MyAuthorsForm(authorController).ShowDialog(this);
        }

        private void AddPluginInformationToolStripMenuItemClick(object sender, EventArgs e)
        {
            new AddPluginInformationForm(
                authorController,
                remotePluginController).ShowDialog(this);
        }

        private void UpdatePluginInformationToolStripMenuItemClick(object sender, EventArgs e)
        {
            new UpdatePluginInformationForm(
                authorController,
                remotePluginController).ShowDialog(this);
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
            var path = Application.LocalUserAppDataPath.Substring(
                    0, Application.LocalUserAppDataPath.LastIndexOf(@"\", StringComparison.Ordinal));

            var file = string.Format("log-{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"));

            Process.Start(Path.Combine(path, file));
        }
    }
}
