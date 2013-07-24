namespace NIHEI.SC4Buddy.View.Application
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Resources;
    using System.Threading;
    using System.Windows.Forms;

    using NIHEI.SC4Buddy.Control;
    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.DataAccess;
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
        private readonly ResourceManager localizationManager;

        private readonly UserFolderController controller;

        private EventLog eventLog = new EventLog(ConfigurationManager.AppSettings["EventLogName"]);

        public Sc4Buddy()
        {
            InitializeComponent();

            localizationManager = new System.ComponentModel.ComponentResourceManager(typeof(Sc4Buddy));
            controller = new UserFolderController(RegistryFactory.UserFolderRegistry);

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
            var selectUserFolderText = localizationManager.GetString("UserFolderComboBox.Text");
            if (UserFolderComboBox.SelectedItem == null
                || UserFolderComboBox.Text.Equals(selectUserFolderText))
            {
                UserFolderComboBox.Text = selectUserFolderText;
                UserFolderComboBox.ForeColor = Color.Gray;
            }
            else
            {
                UserFolderComboBox.ForeColor = Color.Black;
            }
        }

        private void UserFolderComboBoxDropDown(object sender, EventArgs e)
        {
            UserFolderComboBox.ForeColor = Color.Black;
        }

        private void ManageFoldersToolStripMenuItemClick(object sender, EventArgs e)
        {
            new ManageUserFoldersForm().ShowDialog(this);

            RepopulateUserFolderRelatives();
        }

        private void RepopulateUserFolderRelatives()
        {
            UserFolderComboBox.BeginUpdate();
            UserFolderComboBox.Items.Clear();

            var remove = userFoldersToolStripMenuItem.DropDownItems.OfType<UserFolderToolStripMenuItem>().ToList();
            foreach (var item in remove)
            {
                userFoldersToolStripMenuItem.DropDownItems.Remove(item);
            }

            var insertIndex = 0;
            foreach (var userFolder in controller.UserFolders)
            {
                if (userFolder.Id != 1)
                {
                    UserFolderComboBox.Items.Add(new ComboBoxItem<UserFolder>(userFolder.Alias, userFolder));
                }

                if (userFolder.Alias.Equals("?"))
                {
                    userFolder.Alias = LocalizationStrings.GameUserFolderName;
                    controller.Update(userFolder);
                }

                userFoldersToolStripMenuItem.DropDownItems.Insert(insertIndex, new UserFolderToolStripMenuItem(userFolder, UserFolderMenuItemClick));
                insertIndex++;
            }

            UserFolderComboBox.EndUpdate();
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
                eventLog.WriteEntry(ex.Message, EventLogEntryType.FailureAudit);
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
            new UserFolderForm(((UserFolderToolStripMenuItem)sender).UserFolder).ShowDialog(this);
        }

        private void PlayButtonClick(object sender, EventArgs e)
        {
            playButton.Enabled = false;
            playButton.Text = LocalizationStrings.StartingGame;
            playButton.ForeColor = Color.Gray;
            playButton.Update();
            var arguments = SetArguments();

            var gameProcessStartInfo = new ProcessStartInfo
                                          {
                                              FileName =
                                                  Settings.Default.GameLocation + @"\Apps\SimCity 4",
                                              Arguments = string.Join(" ", arguments.ToArray())
                                          };

            var gameLauncher = new GameLauncher(gameProcessStartInfo, Settings.Default.AutoSaveWaitTime);
            var gameLauncherThread = new Thread(gameLauncher.Start) { Name = "SC4Buddy GameLauncher" };

            gameLauncherThread.Start();

            Thread.Sleep(5000);

            playButton.Enabled = true;
            playButton.Text = localizationManager.GetString("playButton.Text");
            playButton.ForeColor = Color.Black;
        }

        private List<string> SetArguments()
        {
            var arguments = new List<string>();
            if (!string.IsNullOrWhiteSpace(Settings.Default.LauncherCpuCount))
            {
                arguments.Add(string.Format("-cpucount:{0}", Settings.Default.LauncherCpuCount));
            }

            if (Settings.Default.LauncherLowCpuPriority)
            {
                arguments.Add("-cpupriority:low");
            }

            if (Settings.Default.LauncherDisableAudio)
            {
                arguments.Add("-audio:off");
            }

            if (Settings.Default.LauncherDisableMusic)
            {
                arguments.Add("-music:off");
            }

            if (Settings.Default.LauncherDisableSound)
            {
                arguments.Add("-sound:off");
            }

            if (Settings.Default.LauncherDisableIME)
            {
                arguments.Add("-ime:disabled");
            }

            if (!string.IsNullOrWhiteSpace(Settings.Default.LauncherCursorColour))
            {
                if (Settings.Default.LauncherCursorColour.Equals("system cursors", StringComparison.OrdinalIgnoreCase))
                {
                    arguments.Add("-customcursors:disabled");
                }
                else
                {
                    switch (Settings.Default.LauncherCursorColour)
                    {
                        case "Disabled":
                            arguments.Add("-cursors:disabled");
                            break;
                        case "Black and white":
                            arguments.Add("-cursors:bw");
                            break;
                        case "16 colours":
                            arguments.Add("-cursors:color16");
                            break;
                        case "256 colours":
                            arguments.Add("-cursors:colour256");
                            break;
                        case "Full colours":
                            arguments.Add("-cursors:fullcolor");
                            break;
                    }
                }
            }

            if (Settings.Default.LauncherCustomResolution)
            {
                arguments.Add("-customresolution:enabled");
            }

            if (!string.IsNullOrWhiteSpace(Settings.Default.LauncherGameMode))
            {
                switch (Settings.Default.LauncherGameMode)
                {
                    case "Window":
                        arguments.Add("-w");
                        break;
                    case "Fullscreen":
                        arguments.Add("-f");
                        break;
                }
            }

            if (Settings.Default.LauncherIgnoreMissingModels)
            {
                arguments.Add("-ignoremissingmodelsdatabugs:on");
            }

            if (Settings.Default.LauncherPauseMinimized)
            {
                arguments.Add("-gp");
            }

            if (Settings.Default.LauncherDisableExceptionHandling)
            {
                arguments.Add("-exceptionhandling:off");
            }

            if (Settings.Default.LauncherDisableBackgroundLoader)
            {
                arguments.Add("-backgroundloader:off");
            }

            if (Settings.Default.LauncherSkipIntro)
            {
                arguments.Add("-intro:off");
            }

            if (Settings.Default.LauncherWriteLog)
            {
                arguments.Add("-writeLog:on");
            }

            if (!string.IsNullOrWhiteSpace(Settings.Default.LauncherLanguage))
            {
                arguments.Add("-l:" + Settings.Default.LauncherLanguage);
            }

            if (!string.IsNullOrWhiteSpace(Settings.Default.LauncherResolution))
            {
                var res = string.Format("-r{0}", Settings.Default.LauncherResolution);
                if (!string.IsNullOrWhiteSpace(Settings.Default.LauncherColourDepth))
                {
                    res += string.Format("x{0}", Settings.Default.LauncherColourDepth.Substring(0, 2));
                }

                arguments.Add(res);
            }

            var selectedUserFolder = UserFolderComboBox.SelectedItem as ComboBoxItem<UserFolder>;

            if (selectedUserFolder != null)
            {
                arguments.Add(string.Format("-userDir:\"{0}\\\"", selectedUserFolder.Value.Path));
            }

            return arguments;
        }

        private void SettingsToolStripMenuItemClick(object sender, EventArgs e)
        {
            new SettingsForm().ShowDialog(this);

            UpdateBackground();
        }

        private void DeveloperToolStripMenuItemClick(object sender, EventArgs e)
        {
            new DeveloperForm().ShowDialog(this);
        }

        private void MyAuthorsToolStripMenuItemClick(object sender, EventArgs e)
        {
            new MyAuthorsForm().ShowDialog(this);
        }

        private void AddPluginInformationToolStripMenuItemClick(object sender, EventArgs e)
        {
            new AddPluginInformationForm().ShowDialog(this);
        }

        private void UpdatePluginInformationToolStripMenuItemClick(object sender, EventArgs e)
        {
            new UpdatePluginInformationForm().ShowDialog(this);
        }

        private void SupportToolStripMenuItemClick(object sender, EventArgs e)
        {
            Process.Start("http://community.simtropolis.com/topic/58814-the-simcity-4-buddy-project/");
        }

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }
    }
}
