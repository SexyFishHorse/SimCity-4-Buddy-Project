namespace NIHEI.SC4Buddy.View.UserFolders
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Irradiated.Sc4Buddy.ApiClient.Model;
    using log4net;

    using NIHEI.SC4Buddy.Configuration;
    using NIHEI.SC4Buddy.Control.Plugins;
    using NIHEI.SC4Buddy.Control.UserFolders;
    using NIHEI.SC4Buddy.DataAccess;
    using NIHEI.SC4Buddy.Localization;
    using NIHEI.SC4Buddy.Model;
    using NIHEI.SC4Buddy.Remote;
    using NIHEI.SC4Buddy.View.Elements;
    using NIHEI.SC4Buddy.View.Helpers;
    using NIHEI.SC4Buddy.View.Plugins;
    using Plugin = NIHEI.SC4Buddy.Model.Plugin;

    public partial class UserFolderForm : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDependencyChecker dependencyChecker;

        private readonly PluginController pluginController;

        private readonly PluginGroupController pluginGroupController;

        private readonly IPluginMatcher pluginMatcher;

        private readonly UserFolder userFolder;

        private readonly IUserFolderRepository userFolderRepository;

        private readonly IUserFolderController userFolderController;

        private Plugin selectedPlugin;

        public UserFolderForm(
            PluginController pluginController,
            PluginGroupController pluginGroupController,
            IUserFolderController userFolderController,
            UserFolder userFolder,
            IPluginMatcher pluginMatcher,
            IDependencyChecker dependencyChecker,
            IUserFolderRepository userFolderRepository)
        {
            this.pluginGroupController = pluginGroupController;
            this.pluginController = pluginController;
            this.userFolderController = userFolderController;
            this.userFolder = userFolder;
            this.pluginMatcher = pluginMatcher;
            this.dependencyChecker = dependencyChecker;
            this.userFolderRepository = userFolderRepository;

            if (!Directory.Exists(userFolder.FolderPath))
            {
                if (userFolder.IsMainFolder)
                {
                    if (userFolder.Alias.Equals("?"))
                    {
                        userFolder.Alias = LocalizationStrings.GameUserFolderName;
                    }

                    userFolder.FolderPath = Settings.Get(Settings.Keys.GameLocation);
                    userFolderRepository.Update(userFolder);
                }
                else
                {
                    throw new DirectoryNotFoundException(
                        "The plugin folder does not exist or you don't have access to one or more of the folders in the path.");
                }
            }

            InitializeComponent();
        }

        private void UserFolderFormLoad(object sender, EventArgs e)
        {
            RepopulateInstalledPluginsListView();

            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                updateInfoForAllPluginsFromServerToolStripMenuItem.Visible = false;
                checkForMissingDependenciesToolStripMenuItem.Visible = false;
            }
            else
            {
                updateInfoForAllPluginsFromServerToolStripMenuItem.Visible = Settings.Get<bool>(Settings.Keys.FetchInformationFromRemoteServer);
                checkForMissingDependenciesToolStripMenuItem.Visible = Settings.Get<bool>(Settings.Keys.AllowCheckForMissingDependencies);
            }
        }

        private void RepopulateInstalledPluginsListView()
        {
            installedPluginsListView.BeginUpdate();
            installedPluginsListView.Items.Clear();
            installedPluginsListView.Groups.Clear();

            foreach (var pluginGroup in pluginGroupController.Groups.Where(x => x.Plugins.Any()))
            {
                installedPluginsListView.Groups.Add(pluginGroup.Id.ToString(), pluginGroup.Name);
            }

            foreach (var plugin in
                pluginController.Plugins.Where(x => x.UserFolderId == userFolder.Id))
            {
                var listViewItem = new PluginListViewItem(
                    plugin,
                    installedPluginsListView.Groups[plugin.PluginGroupId.ToString()]);

                installedPluginsListView.Items.Add(listViewItem);
            }

            installedPluginsListView.EndUpdate();
        }

        private void InstalledPluginsListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItems = installedPluginsListView.SelectedItems;

            if (selectedItems.Count > 0)
            {
                splitContainer1.Panel2Collapsed = false;
                selectedPlugin = ((PluginListViewItem)selectedItems[0]).Plugin;
                nameLabel.Text = selectedPlugin.Name;
                authorLabel.Text = selectedPlugin.Author;
                descriptionRichTextBox.Text = selectedPlugin.Description;
                if (selectedPlugin.Link != null)
                {
                    linkLabel.Text = selectedPlugin.Link.Value;
                }
                else
                {
                    linkLabel.Text = string.Empty;
                }

                uninstallButton.Enabled = true;
                updateInfoButton.Enabled = selectedPlugin.RemotePlugin == null;

                ////reportPluginLinkLabel.Visible = selectedPlugin.RemotePlugin != null;
                reportPluginLinkLabel.Visible = false;
                moveOrCopyButton.Enabled = true;
                disableFilesButton.Enabled = true;

                // TODO: Show reports
                ////if (selectedPlugin.RemotePlugin != null
                ////    && selectedPlugin.RemotePlugin.Reports != null
                ////    && selectedPlugin.RemotePlugin.Reports.Any(x => x.Approved))
                ////{
                ////    var output = new StringBuilder();

                ////    foreach (var report in selectedPlugin.RemotePlugin.Reports
                ////        .Where(x => x.Approved)
                ////        .OrderByDescending(x => x.Date))
                ////    {
                ////        var message = string.Format(
                ////            "[{0}] - {1}",
                ////            report.Date.ToString(CultureInfo.CurrentUICulture.DateTimeFormat),
                ////            report.Body);

                ////        output.AppendLine(message);
                ////        output.AppendLine();
                ////    }

                ////    errorTextBox.Text = output.ToString();
                ////    pluginInfoSplitContainer.Panel2Collapsed = false;
                ////}
                ////else
                ////{
                ////    pluginInfoSplitContainer.Panel2Collapsed = true;
                ////}
                pluginInfoSplitContainer.Panel2Collapsed = true;
            }
            else
            {
                splitContainer1.Panel2Collapsed = true;

                uninstallButton.Enabled = false;
                updateInfoButton.Enabled = false;
                moveOrCopyButton.Enabled = false;
                disableFilesButton.Enabled = false;
            }
        }

        private void UninstallButtonClick(object sender, EventArgs e)
        {
            var confirmation = MessageBox.Show(
                this,
                string.Format(LocalizationStrings.AreYouSureYouWantToUninstall, selectedPlugin.Name),
                LocalizationStrings.ConfirmUninstall,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (confirmation == DialogResult.No)
            {
                return;
            }

            userFolderController.UninstallPlugin(selectedPlugin);
            ClearPluginInformation();
            RepopulateInstalledPluginsListView();
        }

        private void ClearPluginInformation()
        {
            nameLabel.Text = string.Empty;
            authorLabel.Text = string.Empty;
            descriptionRichTextBox.Text = string.Empty;

            uninstallButton.Enabled = false;
            updateInfoButton.Enabled = false;
            splitContainer1.Panel2Collapsed = true;
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void LinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var link = selectedPlugin.Link.Value;
                Log.Info(string.Format("Launching browser: {0}", link));

                Process.Start(link);
            }
            catch (Exception ex)
            {
                Log.Warn("Unable to launch browser", ex);
                MessageBox.Show(
                    this,
                    string.Format(LocalizationStrings.UnableToLaunchBrowser, ex.Message),
                    LocalizationStrings.ErrorDuringLaunchOfBrowser,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void UpdateInfoButtonClick(object sender, EventArgs e)
        {
            var infoDialog = new EnterPluginInformationForm(pluginGroupController)
            {
                Plugin = selectedPlugin
            };
            if (infoDialog.ShowDialog(this) == DialogResult.OK)
            {
                pluginController.SaveChanges();
            }

            RepopulateInstalledPluginsListView();
        }

        private void InstallToolStripMenuItemClick(object sender, EventArgs e)
        {
            var result = selectFilesToInstallDialog.ShowDialog(this);
            if (result == DialogResult.Cancel)
            {
                return;
            }

            var files = selectFilesToInstallDialog.FileNames;

            var confirmResult = MessageBox.Show(
                this,
                string.Format(LocalizationStrings.AreYouSureYouWantToInstallTheSelectedPlugins, files.Count()),
                LocalizationStrings.ConfirmInstallation,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1);

            if (confirmResult == DialogResult.No)
            {
                return;
            }

            new InstallPluginsForm(pluginController, files, userFolder, pluginMatcher).ShowDialog(this);

            RepopulateInstalledPluginsListView();

            if (!NetworkInterface.GetIsNetworkAvailable() || !Settings.Get<bool>(Settings.Keys.AllowCheckForMissingDependencies))
            {
                return;
            }

            var scanForDependencies = MessageBox.Show(
                this,
                LocalizationStrings.WouldYouLikeToScanForMissingDependencies,
                LocalizationStrings.DependencyCheck,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);

            if (scanForDependencies == DialogResult.Yes)
            {
                CheckForMissingDependenciesToolStripMenuItemClick(sender, e);
            }
        }

        private void ScanForNewPluginsToolStripMenuItemClick(object sender, EventArgs e)
        {
            new FolderScannerForm(
                new FolderScannerController(new PluginFileController(EntityFactory.Instance.Entities)),
                pluginController,
                pluginGroupController,
                userFolder,
                pluginMatcher).ShowDialog(this);
            RepopulateInstalledPluginsListView();

            if (!NetworkInterface.GetIsNetworkAvailable() || !Settings.Get<bool>(Settings.Keys.AllowCheckForMissingDependencies))
            {
                return;
            }

            var scanForDependencies = MessageBox.Show(
                this,
                LocalizationStrings.WouldYouLikeToScanForMissingDependencies,
                LocalizationStrings.DependencyCheck,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);

            if (scanForDependencies == DialogResult.Yes)
            {
                CheckForMissingDependenciesToolStripMenuItemClick(sender, e);
            }
        }

        private void ScanForNonpluginFilesToolStripMenuItemClick(object sender, EventArgs e)
        {
            var storageLocation = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Irradiated Games",
                "SimCity 4 Buddy",
                "Configuration");

            var nonPluginFilesScannerUi = new NonPluginFilesScannerUi(storageLocation) { UserFolder = userFolder };

            nonPluginFilesScannerUi.ScanForCandidates();

            if (!nonPluginFilesScannerUi.RemovalCandidateInfos.Any())
            {
                nonPluginFilesScannerUi.ShowThereAreNoEntitiesToRemoveDialog(this);
                return;
            }

            if (!nonPluginFilesScannerUi.ShowConfirmDialog(this))
            {
                return;
            }

            nonPluginFilesScannerUi.RemoveNonPluginFilesAndShowSummary(this);
        }

        private async void UpdateInfoForAllPluginsFromServerToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                await UpdateInfoForAllPluginsFromServer();
            }
            catch (Exception ex)
            {
                Log.Error("Fetch information for plugins error", ex);
                MessageBox.Show(
                    this,
                    LocalizationStrings.ErrorOccuredDuringFetchOfInformationForPlugins + ex.Message,
                    LocalizationStrings.ErrorDuringFetchInformationForPlugins,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private async Task UpdateInfoForAllPluginsFromServer()
        {
            try
            {
                var numUpdated = await userFolderController.UpdateInfoForAllPluginsFromServer(pluginMatcher);
                userFolderController.SaveChanges();
                RepopulateInstalledPluginsListView();

                MessageBox.Show(
                    this,
                    string.Format(LocalizationStrings.InformationForNumPluginsWereUpdated, numUpdated),
                    LocalizationStrings.PluginInformationUpdated,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
            }
            catch (Sc4BuddyClientException ex)
            {
                Log.Warn(ex);
                var message =
                    string.Format(
                        LocalizationStrings.UnableToUpdatePluginsFromServerTheFollowingErrorWasReturned,
                        ex.Message);
                var dialogResult = MessageBox.Show(
                    this,
                    message,
                    LocalizationStrings.ErrorWhileTryingToUpdatePlugins,
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Retry)
                {
                    UpdateInfoForAllPluginsFromServer();
                }
            }
        }

        private void UserFolderFormActivated(object sender, EventArgs e)
        {
            updateInfoForAllPluginsFromServerToolStripMenuItem.Visible = Settings.Get<bool>(Settings.Keys.FetchInformationFromRemoteServer);
        }

        private async void CheckForMissingDependenciesToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                await userFolderController.UpdateInfoForAllPluginsFromServer(pluginMatcher);

                var numRecognizedPlugins = userFolderController.NumberOfRecognizedPlugins(userFolder);

                if (numRecognizedPlugins < 1)
                {
                    MessageBox.Show(
                        this,
                        LocalizationStrings.NoneOfYourPluginsAreRecognizedOnTheCentralServerAndCanThereforeNotBeChecked,
                        LocalizationStrings.NoRecognizablePluginsFound,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                    return;
                }

                var missingDependencies = (await dependencyChecker.CheckDependenciesAsync(userFolder)).ToList();

                if (missingDependencies.Any())
                {
                    var dialog = new MissingDependenciesForm
                    {
                        MissingDependencies = missingDependencies
                    };
                    dialog.ShowDialog(this);
                }
                else
                {
                    var message = string.Format(
                        LocalizationStrings.NumPluginsCheckedForMissingPluginsAndNoneWereMissing,
                        numRecognizedPlugins);

                    MessageBox.Show(
                        this,
                        message,
                        LocalizationStrings.NoDependenciesMissing,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Dependency check error", ex);
                MessageBox.Show(
                    this,
                    LocalizationStrings.ErrorOccuredDuringDependencyCheck + ex.Message,
                    LocalizationStrings.ErrorDuringDependencyCheck,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void MoveOrCopyButtonClick(object sender, EventArgs e)
        {
            var dialog = new MoveOrCopyForm(
                userFolder,
                userFolderController,
                pluginController,
                new PluginFileController(EntityFactory.Instance.Entities),
                userFolderRepository)
            {
                Plugin = selectedPlugin
            };
            dialog.PluginCopied += DialogOnPluginCopied;
            dialog.PluginMoved += DialogOnPluginMoved;
            dialog.ErrorDuringCopyOrMove += DialogOnErrorDuringCopyOrMove;
            dialog.ShowDialog(this);

            RepopulateInstalledPluginsListView();
        }

        private void DialogOnPluginMoved(object sender, EventArgs eventArgs)
        {
            MessageBox.Show(
                this,
                LocalizationStrings.PluginHasBeenSuccessfullyMoved,
                LocalizationStrings.PluginMoved,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void DialogOnErrorDuringCopyOrMove(object sender, EventArgs eventArgs)
        {
            MessageBox.Show(
                this,
                LocalizationStrings.ErrorDuringCopyOrMoveOperation,
                LocalizationStrings.ErrorDuringCopyingOrMovingPlugin,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void DialogOnPluginCopied(object sender, EventArgs eventArgs)
        {
            MessageBox.Show(
                this,
                LocalizationStrings.PluginHasBeenSuccessfullyCopied,
                LocalizationStrings.PluginCopied,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void DisableFilesButtonClick(object sender, EventArgs e)
        {
            var dialog = new QuarantinedPluginFilesForm(
                selectedPlugin,
                new PluginFileController(EntityFactory.Instance.Entities));
            var result = dialog.ShowDialog(this);
        }

        private void ReportPluginLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var dialog = new ReportPluginForm
            {
                Plugin = selectedPlugin.RemotePlugin
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(
                    this,
                    LocalizationStrings.ThePluginHasBeenReportedAnAdministratorWillHaveToApproveItFirst,
                    LocalizationStrings.PluginSuccessfullyReported,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void OpenInFileExplorerToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (Directory.Exists(userFolder.FolderPath))
            {
                Process.Start(userFolder.FolderPath);
            }
            else
            {
                MessageBox.Show(
                    this,
                    string.Format("The directory \"{0}\" doesn't appear to exist.", userFolder.FolderPath),
                    "Directory not found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }
    }
}
